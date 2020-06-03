import sys
import numpy as np
import cv2 as cv


class Camera:
    __WIDTH = 640
    __HEIGHT = 480
    __DEPTH_WIDTH = __WIDTH // 8
    __DEPTH_HEIGHT = __HEIGHT // 8

    def __init__(self, racecar):
        self.__racecar = racecar
        self.__color_image = None
        self.__is_color_image_current = False
        self.__depth_image = None
        self.__is_depth_image_current = False

    def get_image(self):
        if not self.__is_color_image_current:
            self.__update_color_image()
            self.__is_color_image_current = True

        return self.__color_image

    def get_depth_image(self):
        if not self.__is_depth_image_current:
            self.__update_depth_image()
            self.__is_depth_image_current = False

        return self.__depth_image

    def get_width(self):
        return self.__WIDTH

    def get_height(self):
        return self.__HEIGHT

    def __update(self):
        self.__is_color_image_current = False
        self.__is_depth_image_current = False

    def __update_color_image(self):
        # Ask for a the current color image
        self.__racecar._Racecar__send_header(self.__racecar.Header.camera_get_image)

        # Read the color image as 32 packets
        raw_bytes = bytes()
        for i in range(0, 32):
            raw_bytes += self.__racecar._Racecar__receive_data(
                self.__WIDTH * self.__HEIGHT * 4 // 32
            )
            self.__racecar._Racecar__send_header(self.__racecar.Header.python_send_next)

        self.__color_image = np.frombuffer(raw_bytes, dtype=np.uint8)
        self.__color_image = np.reshape(
            self.__color_image, (self.__HEIGHT, self.__WIDTH, 4), "C"
        )

        self.__color_image = cv.cvtColor(self.__color_image, cv.COLOR_RGB2BGR)

    def __update_depth_image(self):
        self.__racecar._Racecar__send_header(
            self.__racecar.Header.camera_get_depth_image
        )
        raw_bytes = self.__racecar._Racecar__receive_data(
            self.__DEPTH_WIDTH * self.__DEPTH_HEIGHT * 4
        )
        self.__depth_image = np.frombuffer(raw_bytes, dtype=np.float32)
        self.__depth_image = np.reshape(
            self.__depth_image, (self.__DEPTH_HEIGHT, self.__DEPTH_WIDTH), "C"
        )

        self.__depth_image = cv.resize(
            self.__depth_image,
            (self.__WIDTH, self.__HEIGHT),
            interpolation=cv.INTER_CUBIC,
        )

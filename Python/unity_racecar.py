import math
import struct
import socket
import time
import sys
from enum import IntEnum

import unity_camera
import unity_controller
import unity_display
import unity_drive
import unity_lidar
import unity_physics

class Racecar:
    __IP = "127.0.0.1"
    __UNITY_PORT = 5065
    __PYTHON_PORT = 5066
    __SOCKET = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    class Header(IntEnum):
        """
        The buttons on the controller
        """
        error = 0
        unity_start = 1
        unity_update = 2
        unity_exit = 3
        python_finished = 4
        python_send_next = 5
        racecar_go = 6
        racecar_set_start_update = 7
        racecar_get_delta_time = 8
        racecar_set_update_slow_time = 9
        camera_get_image = 10
        camera_get_depth_image = 11
        camera_get_width = 12
        camera_get_height = 13
        controller_is_down = 14
        controller_was_pressed = 15
        controller_was_released = 16
        controller_get_trigger = 17
        controller_get_joystick = 18
        display_show_image = 19
        drive_set_speed_angle = 20
        drive_stop = 21
        drive_set_max_speed_scale_factor = 22
        gpio_pin_mode = 23
        gpio_pin_write = 24
        lidar_get_length = 25
        lidar_get_ranges = 26
        physics_get_linear_acceleration = 27
        physics_get_angular_velocity = 28

    def __send_header(self, function_code):
        self.__send_data(struct.pack("B", function_code.value))

    def __send_data(self, data):
        Racecar.__SOCKET.sendto(
            data, (Racecar.__IP, Racecar.__UNITY_PORT)
        )

    def __receive_data(self, buffer_size=4):
        data, _ = Racecar.__SOCKET.recvfrom(buffer_size)
        return data

    def __init__(self):
        self.camera = unity_camera.Camera(self)
        self.controller = unity_controller.Controller(self)
        self.display = unity_display.Display()
        self.drive = unity_drive.Drive(self)
        self.physics = unity_physics.Physics(self)
        self.lidar = unity_lidar.Lidar(self)

        self.start = None
        self.update = None
        self.update_slow = None
        self.update_slow_time = 1

        self.__SOCKET.bind((self.__IP, self.__PYTHON_PORT))

    def go(self):
        print(">> Python script loaded, please enter user program mode in Unity")
        while True:
            data, _ = self.__SOCKET.recvfrom(256)
            header = int.from_bytes(data, sys.byteorder)

            response = self.Header.error
            if header == self.Header.unity_start.value:
                self.start()
                response = self.Header.python_finished
            elif header == self.Header.unity_update.value:
                self.update()
                self.__update_modules()
                response = self.Header.python_finished
            elif header == self.Header.unity_exit.value:
                print(">> Exit command received from Unity")
                break
            else:
                print(">> Error: unexpected packet from Unity", header)

            self.__send_header(response)

    def set_start_update(self, start, update, update_slow = None):
        self.start = start
        self.update = update
        self.update_slow = update_slow

    def get_delta_time(self):
        self.__send_header(self.Header.racecar_get_delta_time)
        [value] = struct.unpack("f", self.__receive_data())
        return value

    def set_update_slow_time(self, update_slow_time):
        self.update_slow_time = update_slow_time

    def __update_modules(self):
        self.camera._Camera__update()
        self.lidar._Lidar__update()

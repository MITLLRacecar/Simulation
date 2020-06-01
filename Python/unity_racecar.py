import math
import struct
import socket
import time
import sys
from enum import IntEnum

import unity_camera
import unity_controller
import unity_drive

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
        racecar_go = 5
        racecar_set_start_update = 6
        racecar_get_delta_time = 7
        racecar_set_update_slow_time = 8
        camera_get_image = 9
        camera_get_depth_image = 10
        camera_get_width = 11
        camera_get_height = 12
        controller_is_down = 13
        controller_was_pressed = 14
        controller_was_released = 15
        controller_get_trigger = 16
        controller_get_joystick = 17
        display_show_image = 18
        drive_set_speed_angle = 19
        drive_stop = 20
        drive_set_max_speed_scale_factor = 21
        gpio_pin_mode = 22
        gpio_pin_write = 23
        lidar_get_length = 24
        lidar_get_ranges = 25
        physics_get_linear_acceleration = 26
        physics_get_angular_velocity = 27
        sound_set_speaker = 28
        sound_set_mic = 29
        sound_set_output_stream = 30
        sound_set_input_stream = 31
        sound_play_audio = 32
        sound_record_audio = 33
        sound_play = 34
        sound_rec = 35
        sound_set_file = 36
        sound_list_devices = 37

    def __send_header(self, function_code):
        self.__send_data(struct.pack("B", function_code.value))

    def __send_data(self, data):
        Racecar.__SOCKET.sendto(
            data, (Racecar.__IP, Racecar.__UNITY_PORT)
        )
        print("data sent")

    def __receive_data(self, buffer_size=4):
        data, _ = Racecar.__SOCKET.recvfrom(buffer_size)
        return data

    def __init__(self):
        self.camera = unity_camera.Camera(self)
        self.controller = unity_controller.Controller(self)
        self.drive = unity_drive.Drive(self)

        self.start = None
        self.update = None

        self.__SOCKET.bind((self.__IP, self.__PYTHON_PORT))

    def go(self):
        print(">> Python script ready, please start Unity...")
        while True:
            data, _ = self.__SOCKET.recvfrom(256)
            header = int.from_bytes(data, sys.byteorder)
            print("data received:", header)

            response = self.Header.error
            if header == self.Header.unity_start.value:
                self.start()
                response = self.Header.python_finished
            elif header == self.Header.unity_update.value:
                self.update()
                response = self.Header.python_finished
            elif header == self.Header.unity_exit.value:
                break
            else:
                print("Unexpected packet from Unity")

            self.__send_header(response)

    def set_start_update(self, start, update):
        self.start = start
        self.update = update

rc = Racecar()

def start():
    print("start")

def update():
    print("update")
    rc.drive.set_speed_angle(1, -0.5)

rc.set_start_update(start, update)
rc.go()

# while True:
#     # send a drive instruction
#     data = struct.pack("Bff", Header.drive_set_speed_angle.value, 1, -0.5)

#     __FUNCTION_SOCKET.sendto(data, (UDP_IP, UDP_FUNCTION_PORT))

#     # ask for image height
#     data = struct.pack("B", Header.camera_get_height.value)
#     __FUNCTION_SOCKET.sendto(data, (UDP_IP, UDP_FUNCTION_PORT))

#     receivedData, addr = __FUNCTION_SOCKET.recvfrom(16)
#     print("get_height returned:", int.from_bytes(receivedData, sys.byteorder))

#     time.sleep(2)

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
    __UDP_IP = "127.0.0.1"
    __UDP_FUNCTION_PORT = 5065
    __UDP_CLOCK_PORT = 5066
    __FUNCTION_SOCKET = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    __CLOCK_SOCKET = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    class FunctionCode(IntEnum):
        """
        The buttons on the controller
        """

        go = 0
        set_start_update = 1
        get_delta_time = 2
        set_update_slow_time = 3
        camera_get_image = 4
        camera_get_depth_image = 5
        camera_get_width = 6
        camera_get_height = 7
        controller_is_down = 8
        controller_was_pressed = 9
        controller_was_released = 10
        controller_get_trigger = 11
        controller_get_joystick = 12
        display_show_image = 13
        drive_set_speed_angle = 14
        drive_stop = 15
        drive_set_max_speed_scale_factor = 16
        gpio_pin_mode = 17
        gpio_pin_write = 18
        lidar_get_length = 19
        lidar_get_ranges = 20
        physics_get_linear_acceleration = 21
        physics_get_angular_velocity = 22
        sound_set_speaker = 23
        sound_set_mic = 24
        sound_set_output_stream = 25
        sound_set_input_stream = 26
        sound_play_audio = 27
        sound_record_audio = 28
        sound_play = 29
        sound_rec = 30
        sound_set_file = 31
        sound_list_devices = 32

    def __send_call(self, function_code):
        self.__send_data(struct.pack("B", function_code.value))

    def __send_data(self, data):
        Racecar.__FUNCTION_SOCKET.sendto(
            data, (Racecar.__UDP_IP, Racecar.__UDP_FUNCTION_PORT)
        )

    def __receive_data(self, buffer_size=4):
        data, _ = Racecar.__FUNCTION_SOCKET.recvfrom(buffer_size)
        return data

    def __init__(self):
        self.camera = unity_camera.Camera(self)
        self.controller = unity_controller.Controller(self)
        self.drive = unity_drive.Drive(self)


rc = Racecar()

while True:
    rc.drive.set_speed_angle(1, -0.5)
    print(rc.controller.is_down(rc.controller.Button.A))
    time.sleep(2)

# while True:
#     # send a drive instruction
#     data = struct.pack("Bff", FunctionCode.drive_set_speed_angle.value, 1, -0.5)

#     __FUNCTION_SOCKET.sendto(data, (UDP_IP, UDP_FUNCTION_PORT))

#     # ask for image height
#     data = struct.pack("B", FunctionCode.camera_get_height.value)
#     __FUNCTION_SOCKET.sendto(data, (UDP_IP, UDP_FUNCTION_PORT))

#     receivedData, addr = __FUNCTION_SOCKET.recvfrom(16)
#     print("get_height returned:", int.from_bytes(receivedData, sys.byteorder))

#     time.sleep(2)

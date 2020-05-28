import math
import struct
import socket
import time
import sys
from enum import IntEnum


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
    set_max_speed_scale_factor = 16
    pin_mode = 17
    pin_write = 18
    get_length = 19
    get_ranges = 20
    get_linear_acceleration = 21
    get_angular_velocity = 22
    set_speaker = 23
    set_mic = 24
    set_output_stream = 25
    set_input_stream = 26
    play_audio = 27
    record_audio = 28
    play = 29
    rec = 30
    set_file = 31
    list_devices = 32

UDP_IP = "127.0.0.1"
UDP_FUNCTION_PORT = 5065
UDP_CLOCK_PORT = 5066

function_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
clock_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

while True:
    # send a drive instruction
    data = struct.pack("Bff", FunctionCode.drive_set_speed_angle.value, 1, 0.5)

    function_socket.sendto(data, (UDP_IP, UDP_FUNCTION_PORT))

    # ask for image height
    data = struct.pack("B", FunctionCode.camera_get_height.value)
    function_socket.sendto(data, (UDP_IP, UDP_FUNCTION_PORT))

    receivedData, addr = function_socket.recvfrom(16)
    print("get_height returned:", int.from_bytes(receivedData, sys.byteorder))

    time.sleep(2)

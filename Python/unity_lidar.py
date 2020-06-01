import sys
import struct
import numpy as np
import binascii

class Lidar:
    __LENGTH = 720

    def __init__(self, racecar):
        self.__racecar = racecar
        self.__ranges = None
        self.__is_current = False

    def get_length(self):
        return self.__LENGTH

    def get_ranges(self):
        if not self.__is_current:
            self.__racecar._Racecar__send_header(self.__racecar.Header.lidar_get_ranges)
            raw_bytes = self.__racecar._Racecar__receive_data(self.__LENGTH * 4)
            self.__ranges = np.frombuffer(raw_bytes, dtype=np.float32)
            self.__is_current = True
        return self.__ranges

    def __update(self):
        self.__is_current = False

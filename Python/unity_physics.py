import sys
import struct


class Physics:
    def __init__(self, racecar):
        self.__racecar = racecar

    def get_linear_acceleration(self):
        self.__racecar._Racecar__send_header(
            self.__racecar.Header.physics_get_linear_acceleration
        )
        values = struct.unpack("fff", self.__racecar._Racecar__receive_data(12))
        return values

    def get_angular_velocity(self):
        self.__racecar._Racecar__send_header(
            self.__racecar.Header.physics_get_linear_acceleration
        )
        values = struct.unpack("fff", self.__racecar._Racecar__receive_data(12))
        return values

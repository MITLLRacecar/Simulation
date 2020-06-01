import sys
import struct
from enum import IntEnum

class Controller:

    class Button(IntEnum):
        """
        The buttons on the controller
        """

        A = 0  # A button
        B = 1  # B button
        X = 2  # X button
        Y = 3  # Y button
        LB = 4  # Left bumper
        RB = 5  # Right bumper
        LJOY = 6  # Left joystick button
        RJOY = 7  # Right joystick button

    class Trigger(IntEnum):
        """
        The triggers on the controller
        """

        LEFT = 0
        RIGHT = 1

    class Joystick(IntEnum):
        """
        The joysticks on the controller
        """

        LEFT = 0
        RIGHT = 1

    def __init__(self, racecar):
        self.__racecar = racecar

    def is_down(self, button):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB", self.__racecar.Header.controller_is_down.value, button.value
            )
        )
        return bool.from_bytes(self.__racecar._Racecar__receive_data(), sys.byteorder)

    def was_pressed(self, button):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.Header.controller_was_pressed.value,
                button.value,
            )
        )
        return bool.from_bytes(self.__racecar._Racecar__receive_data(), sys.byteorder)

    def was_released(self, button):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.Header.controller_was_pressed.value,
                button.value,
            )
        )
        return bool.from_bytes(self.__racecar._Racecar__receive_data(), sys.byteorder)

    def get_trigger(self, trigger):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.Header.controller_get_trigger.value,
                trigger.value,
            )
        )
        [value] = struct.unpack("f", self.__racecar._Racecar__receive_data())
        return value

    def get_joystick(self, joystick):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.Header.controller_get_joystick.value,
                joystick.value,
            )
        )
        return struct.unpack("ff", self.__racecar._Racecar__receive_data(8))

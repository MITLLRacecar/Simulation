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

    def __init__(self, racecar):
        self.__racecar = racecar

    def is_down(self, button):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB", self.__racecar.FunctionCode.controller_is_down.value, button.value
            )
        )
        return bool.from_bytes(self.__racecar._Racecar__receive_data())

    def was_pressed(self, button):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.FunctionCode.controller_was_pressed.value,
                button.value,
            )
        )
        return bool.from_bytes(self.__racecar._Racecar__receive_data())

    def was_released(self, button):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.FunctionCode.controller_was_pressed.value,
                button.value,
            )
        )
        return bool.from_bytes(self.__racecar._Racecar__receive_data())

    def get_trigger(self, trigger):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "BB",
                self.__racecar.FunctionCode.controller_was_pressed.value,
                trigger.value,
            )
        )
        [value] = struct.unpack("f", self.__racecar.Racecar._Racecar__receive_data())
        return value

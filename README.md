SilentRoomController
========================
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Build status](https://ci.appveyor.com/api/projects/status/3n467nvo92t7aoj6?svg=true)](https://ci.appveyor.com/project/keesvv/silentroomcontroller)
[![CodeFactor](https://www.codefactor.io/repository/github/keesvv/silentroomcontroller/badge)](https://www.codefactor.io/repository/github/keesvv/silentroomcontroller)

💡 Control your Philips Hue lights using a command-line interface.

![Screenshot](https://github.com/keesvv/silentroomcontroller/raw/master/img/screenshots/screenshot.PNG)

> [Hyper](https://github.com/zeit/hyper) is used as terminal application shown in the image.
> The corresponding theme is called [hyperterm-atom-dark](https://www.npmjs.com/package/hyperterm-atom-dark).

**What’s in the box?**

-   Command-line interface

-   Support for registering users

-   A setup for installing SilentRoomController

-   A bunch of light commands

-   Support for commanding multiple lights at a time

-   Listing for available lights
 

**Supported Commands**

-   `[1] COMMAND_ON`: Turns on the specified light.

-   `[2] COMMAND_OFF`: Turns of the specified light.

-   `[3] COMMAND_TOGGLE`: Toggles the specified light.

-   `[4] COMMAND_SET_BRIGHTNESS`: Sets the brightness.

-   `[5] COMMAND_SET_HUE`: Sets the Hue color value.

-   `[6] COMMAND_SET_SATURATION`: Sets the color saturation.

-   `[7] COMMAND_ENABLE_COLORLOOP`: Enables the color-loop effect.

-   `[8] COMMAND_DISABLE_COLORLOOP`: Disables the color-loop effect.

 

# Command-line Examples

First off, you need to specify which lights you want to target. Let’s say that
you have two lights connected to your bridge:

`[1] Living Room`

`[2] Bedroom`

If you want to target `[1]`, you could type:

`SilentRoomController.exe -id 1 -command <command> [command_args]`

You can also specify multiple lights as a target. For example, if you want both
lights `[1]` and `[2]` as a target. To do so, type the following:

`SilentRoomController.exe -id 1,2 -command <command> [command_args]` 

Here comes the command part. So now that you have specified which light(s) you
want as a target, you can choose a supported command. Commands are written
above. Let’s say we now want lights `[1]` and `[2]` to turn on, type the
following:

`SilentRoomController.exe -id 1,2 -command 1`

The same goes for turning off both lights:

`SilentRoomController.exe -id 1,2 -command 2`

You might have noticed the `[command_args]` parameter. This is for the commands
which contains the ‘(\*)’ symbol. This stands for: command parameter required.
This means that it needs another argument within the arguments. For example, the
`COMMAND_SET_BRIGHTNESS` command needs another argument, which specified the
brightness itself.

Let’s say you want the light to be having brightness 100, you type the
following:

`SilentRoomController.exe -id 1 -command 4 100`

This is the end of the examples. If you’d like to know more about using
SilentRoomController 2.0, consider sending me a message.

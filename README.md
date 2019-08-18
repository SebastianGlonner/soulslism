Soulslism
=========

# clion

Example project on how to configure clion to execute scons and use catch unit test library
https://github.com/SebastianGlonner/cpp-playground-scons

# gdnative

## auto reload to prevent access error when recompiling

in the editor is a checkbox at gdnative properties window which allows to
reload the library


# Debuging

C++ debuging is possible with Visual Studio only at the time of writing.
You need to:
 - change the library from shared to static
 - create the visual studio projects with scons msvs=true
 - open the solutions file and set your breakpoints

Issues:
 - do not execute the building with VS-Studio, only execute (it might be possible to fix the error happening executing the scons build in visual studio)
 - for the purpose of debuging there is a programm test which uses catch unit test library

Soulslism
=========


# Debuging

C++ debuging is possible with Visual Studio only at the time of writing.
You need to:
 - change the library from shared to static
 - create the visual studio projects with scons msvs=true
 - open the solutions file and set your breakpoints

Issues:
 - do not execute the building with VS-Studio, only execute (it might be possible to fix the error happening executing the scons build in visual studio)
 - for the purpose of debuging there is a programm test which uses catch unit test library

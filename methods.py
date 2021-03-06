import os


def build_res_file(target, source, env):

    if (env["bits"] == "32"):
        cmdbase = env['mingw_prefix_32']
    else:
        cmdbase = env['mingw_prefix_64']
    cmdbase = cmdbase + 'windres --include-dir . '
    import subprocess
    for x in range(len(source)):
        cmd = cmdbase + '-i ' + str(source[x]) + ' -o ' + str(target[x])
        try:
            out = subprocess.Popen(cmd, shell=True, stderr=subprocess.PIPE).communicate()
            if len(out[1]):
                return 1
        except:
            return 1
    return 0

def configure_for_windows(env):

    mingw32 = ""
    mingw64 = ""
    if (os.name == "posix"):
        mingw32 = "i686-w64-mingw32-"
        mingw64 = "x86_64-w64-mingw32-"

    if (os.getenv("MINGW32_PREFIX")):
        mingw32 = os.getenv("MINGW32_PREFIX")
    if (os.getenv("MINGW64_PREFIX")):
        mingw64 = os.getenv("MINGW64_PREFIX")

    env['mingw_prefix_32'] = mingw32
    env['mingw_prefix_64'] = mingw64

    if os.getenv("VCINSTALLDIR") and not env["use_mingw"]:
        # Manual setup of MSVC
        setup_msvc_manual(env)
        env.msvc = True
        manual_msvc_config = True
    elif env.get('MSVC_VERSION', '') and not env["use_mingw"]:
        setup_msvc_auto(env)
        env.msvc = True
        manual_msvc_config = False
    else:
        configure_mingw(env)
        env.msvc = False

def setup_msvc_manual(env):
    """Set up env to use MSVC manually, using VCINSTALLDIR"""
    if (env["bits"] != "default"):
        print("""
            Bits argument is not supported for MSVC compilation. Architecture depends on the Native/Cross Compile Tools Prompt/Developer Console
            (or Visual Studio settings) that is being used to run SCons. As a consequence, bits argument is disabled. Run scons again without bits
            argument (example: scons p=windows) and SCons will attempt to detect what MSVC compiler will be executed and inform you.
            """)
        raise SCons.Errors.UserError("Bits argument should not be used when using VCINSTALLDIR")

    # Force bits arg
    # (Actually msys2 mingw can support 64-bit, we could detect that)
    env["bits"] = "32"
    env["x86_libtheora_opt_vc"] = True

    # find compiler manually
    compiler_version_str = detect_visual_c_compiler_version(env['ENV'])
    print("Found MSVC compiler: " + compiler_version_str)

    # If building for 64bit architecture, disable assembly optimisations for 32 bit builds (theora as of writing)... vc compiler for 64bit can not compile _asm
    if(compiler_version_str == "amd64" or compiler_version_str == "x86_amd64"):
        env["bits"] = "64"
        env["x86_libtheora_opt_vc"] = False
        print("Compiled program architecture will be a 64 bit executable (forcing bits=64).")
    elif (compiler_version_str == "x86" or compiler_version_str == "amd64_x86"):
        print("Compiled program architecture will be a 32 bit executable. (forcing bits=32).")
    else:
        print("Failed to manually detect MSVC compiler architecture version... Defaulting to 32bit executable settings (forcing bits=32). Compilation attempt will continue, but SCons can not detect for what architecture this build is compiled for. You should check your settings/compilation setup, or avoid setting VCINSTALLDIR.")


def detect_visual_c_compiler_version(tools_env):
    # tools_env is the variable scons uses to call tools that execute tasks, SCons's env['ENV'] that executes tasks...
    # (see the SCons documentation for more information on what it does)...
    # in order for this function to be well encapsulated i choose to force it to receive SCons's TOOLS env (env['ENV']
    # and not scons setup environment (env)... so make sure you call the right environment on it or it will fail to detect
    # the proper vc version that will be called

    # There is no flag to give to visual c compilers to set the architecture, ie scons bits argument (32,64,ARM etc)
    # There are many different cl.exe files that are run, and each one compiles & links to a different architecture
    # As far as I know, the only way to figure out what compiler will be run when Scons calls cl.exe via Program()
    # is to check the PATH variable and figure out which one will be called first. Code below does that and returns:
    # the following string values:

    # ""              Compiler not detected
    # "amd64"         Native 64 bit compiler
    # "amd64_x86"     64 bit Cross Compiler for 32 bit
    # "x86"           Native 32 bit compiler
    # "x86_amd64"     32 bit Cross Compiler for 64 bit

    # There are other architectures, but Godot does not support them currently, so this function does not detect arm/amd64_arm
    # and similar architectures/compilers

    # Set chosen compiler to "not detected"
    vc_chosen_compiler_index = -1
    vc_chosen_compiler_str = ""

    # Start with Pre VS 2017 checks which uses VCINSTALLDIR:
    if 'VCINSTALLDIR' in tools_env:
        # print("Checking VCINSTALLDIR")

        # find() works with -1 so big ifs below are needed... the simplest solution, in fact
        # First test if amd64 and amd64_x86 compilers are present in the path
        vc_amd64_compiler_detection_index = tools_env["PATH"].find(tools_env["VCINSTALLDIR"] + "BIN\\amd64;")
        if(vc_amd64_compiler_detection_index > -1):
            vc_chosen_compiler_index = vc_amd64_compiler_detection_index
            vc_chosen_compiler_str = "amd64"

        vc_amd64_x86_compiler_detection_index = tools_env["PATH"].find(tools_env["VCINSTALLDIR"] + "BIN\\amd64_x86;")
        if(vc_amd64_x86_compiler_detection_index > -1
           and (vc_chosen_compiler_index == -1
                or vc_chosen_compiler_index > vc_amd64_x86_compiler_detection_index)):
            vc_chosen_compiler_index = vc_amd64_x86_compiler_detection_index
            vc_chosen_compiler_str = "amd64_x86"

        # Now check the 32 bit compilers
        vc_x86_compiler_detection_index = tools_env["PATH"].find(tools_env["VCINSTALLDIR"] + "BIN;")
        if(vc_x86_compiler_detection_index > -1
           and (vc_chosen_compiler_index == -1
                or vc_chosen_compiler_index > vc_x86_compiler_detection_index)):
            vc_chosen_compiler_index = vc_x86_compiler_detection_index
            vc_chosen_compiler_str = "x86"

        vc_x86_amd64_compiler_detection_index = tools_env["PATH"].find(tools_env['VCINSTALLDIR'] + "BIN\\x86_amd64;")
        if(vc_x86_amd64_compiler_detection_index > -1
           and (vc_chosen_compiler_index == -1
                or vc_chosen_compiler_index > vc_x86_amd64_compiler_detection_index)):
            vc_chosen_compiler_index = vc_x86_amd64_compiler_detection_index
            vc_chosen_compiler_str = "x86_amd64"

    # and for VS 2017 and newer we check VCTOOLSINSTALLDIR:
    if 'VCTOOLSINSTALLDIR' in tools_env:

        # Newer versions have a different path available
        vc_amd64_compiler_detection_index = tools_env["PATH"].upper().find(tools_env['VCTOOLSINSTALLDIR'].upper() + "BIN\\HOSTX64\\X64;")
        if(vc_amd64_compiler_detection_index > -1):
            vc_chosen_compiler_index = vc_amd64_compiler_detection_index
            vc_chosen_compiler_str = "amd64"

        vc_amd64_x86_compiler_detection_index = tools_env["PATH"].upper().find(tools_env['VCTOOLSINSTALLDIR'].upper() + "BIN\\HOSTX64\\X86;")
        if(vc_amd64_x86_compiler_detection_index > -1
           and (vc_chosen_compiler_index == -1
                or vc_chosen_compiler_index > vc_amd64_x86_compiler_detection_index)):
            vc_chosen_compiler_index = vc_amd64_x86_compiler_detection_index
            vc_chosen_compiler_str = "amd64_x86"

        vc_x86_compiler_detection_index = tools_env["PATH"].upper().find(tools_env['VCTOOLSINSTALLDIR'].upper() + "BIN\\HOSTX86\\X86;")
        if(vc_x86_compiler_detection_index > -1
           and (vc_chosen_compiler_index == -1
                or vc_chosen_compiler_index > vc_x86_compiler_detection_index)):
            vc_chosen_compiler_index = vc_x86_compiler_detection_index
            vc_chosen_compiler_str = "x86"

        vc_x86_amd64_compiler_detection_index = tools_env["PATH"].upper().find(tools_env['VCTOOLSINSTALLDIR'].upper() + "BIN\\HOSTX86\\X64;")
        if(vc_x86_amd64_compiler_detection_index > -1
           and (vc_chosen_compiler_index == -1
                or vc_chosen_compiler_index > vc_x86_amd64_compiler_detection_index)):
            vc_chosen_compiler_index = vc_x86_amd64_compiler_detection_index
            vc_chosen_compiler_str = "x86_amd64"

    return vc_chosen_compiler_str

def find_visual_c_batch_file(env):
    from SCons.Tool.MSCommon.vc import get_default_version, get_host_target, find_batch_file

    version = get_default_version(env)
    (host_platform, target_platform,req_target_platform) = get_host_target(env)
    return find_batch_file(env, version, host_platform, target_platform)[0]


def setup_msvc_auto(env):
    """Set up MSVC using SCons's auto-detection logic"""

    # If MSVC_VERSION is set by SCons, we know MSVC is installed.
    # But we may want a different version or target arch.

    # The env may have already been set up with default MSVC tools, so
    # reset a few things so we can set it up with the tools we want.
    # (Ideally we'd decide on the tool config before configuring any
    # environment, and just set the env up once, but this function runs
    # on an existing env so this is the simplest way.)
    env['MSVC_SETUP_RUN'] = False # Need to set this to re-run the tool
    env['MSVS_VERSION'] = None
    env['MSVC_VERSION'] = None
    env['TARGET_ARCH'] = None
    if env['bits'] != 'default':
        env['TARGET_ARCH'] = {'32': 'x86', '64': 'x86_64'}[env['bits']]
    if env.has_key('msvc_version'):
           env['MSVC_VERSION'] = env['msvc_version']
    env.Tool('msvc')
    env.Tool('mssdk')                      # we want the MS SDK
    # Note: actual compiler version can be found in env['MSVC_VERSION'], e.g. "14.1" for VS2015
    # Get actual target arch into bits (it may be "default" at this point):
    if env['TARGET_ARCH'] in ('amd64', 'x86_64'):
        env['bits'] = '64'
    else:
        env['bits'] = '32'
    print(" Found MSVC version %s, arch %s, bits=%s" % (env['MSVC_VERSION'], env['TARGET_ARCH'], env['bits']))
    if env['TARGET_ARCH'] in ('amd64', 'x86_64'):
        env["x86_libtheora_opt_vc"] = False


def configure_mingw(env):
    # Workaround for MinGW. See:
    # http://www.scons.org/wiki/LongCmdLinesOnWin32
    # env.use_windows_spawn_fix()

    ## Build type

    if (env["target"] == "release"):
        env.Append(CCFLAGS=['-msse2'])

        if (env["optimize"] == "speed"): #optimize for speed (default)
            if (env["bits"] == "64"):
                env.Append(CCFLAGS=['-O3'])
            else:
                env.Append(CCFLAGS=['-O2'])
        else: #optimize for size
            env.Prepend(CCFLAGS=['-Os'])


        env.Append(LINKFLAGS=['-Wl,--subsystem,windows'])

        if (env["debug_symbols"] == "yes"):
           env.Prepend(CCFLAGS=['-g1'])
        if (env["debug_symbols"] == "full"):
           env.Prepend(CCFLAGS=['-g2'])

    elif (env["target"] == "release_debug"):
        env.Append(CCFLAGS=['-O2', '-DDEBUG_ENABLED'])
        if (env["debug_symbols"] == "yes"):
           env.Prepend(CCFLAGS=['-g1'])
        if (env["debug_symbols"] == "full"):
           env.Prepend(CCFLAGS=['-g2'])
        if (env["optimize"] == "speed"): #optimize for speed (default)
           env.Append(CCFLAGS=['-O2'])
        else: #optimize for size
           env.Prepend(CCFLAGS=['-Os'])

    elif (env["target"] == "debug"):
        env.Append(CCFLAGS=['-g3', '-DDEBUG_ENABLED', '-DDEBUG_MEMORY_ENABLED'])

    ## Compiler configuration

    if (os.name == "nt"):
        env['ENV']['TMP'] = os.environ['TMP']  # way to go scons, you can be so stupid sometimes
    else:
        env["PROGSUFFIX"] = env["PROGSUFFIX"] + ".exe"  # for linux cross-compilation

    if (env["bits"] == "default"):
        if (os.name == "nt"):
            env["bits"] = "64" if "PROGRAMFILES(X86)" in os.environ else "32"
        else: # default to 64-bit on Linux
            env["bits"] = "64"

    mingw_prefix = ""

    if (env["bits"] == "32"):
        env.Append(LINKFLAGS=['-static'])
        env.Append(LINKFLAGS=['-static-libgcc'])
        env.Append(LINKFLAGS=['-static-libstdc++'])
        mingw_prefix = env["mingw_prefix_32"]
    else:
        env.Append(LINKFLAGS=['-static'])
        mingw_prefix = env["mingw_prefix_64"]

    env["CC"] = mingw_prefix + "gcc"
    env['AS'] = mingw_prefix + "as"
    env['CXX'] = mingw_prefix + "g++"
    env['AR'] = mingw_prefix + "gcc-ar"
    env['RANLIB'] = mingw_prefix + "gcc-ranlib"
    env['LINK'] = mingw_prefix + "g++"
    env["x86_libtheora_opt_gcc"] = True

    if env['use_lto']:
        env.Append(CCFLAGS=['-flto'])
        env.Append(LINKFLAGS=['-flto=' + str(env.GetOption("num_jobs"))])


    ## Compile flags

    env.Append(CCFLAGS=['-DWINDOWS_ENABLED', '-mwindows'])
    env.Append(CCFLAGS=['-DOPENGL_ENABLED'])
    env.Append(CCFLAGS=['-DWASAPI_ENABLED'])
    env.Append(CCFLAGS=['-DWINMIDI_ENABLED'])
    env.Append(CCFLAGS=['-DWINVER=%s' % env['target_win_version'], '-D_WIN32_WINNT=%s' % env['target_win_version']])
    # env.Append(LIBS=['mingw32', 'opengl32', 'dsound', 'ole32', 'd3d9', 'winmm', 'gdi32', 'iphlpapi', 'shlwapi', 'wsock32', 'ws2_32', 'kernel32', 'oleaut32', 'dinput8', 'dxguid', 'ksuser', 'imm32', 'bcrypt','avrt'])

    env.Append(CPPFLAGS=['-DMINGW_ENABLED'])

    # resrc
    env.Append(BUILDERS={'RES': env.Builder(action=build_res_file, suffix='.o', src_suffix='.rc')})

def buildMsvs(env, vsSrcs, vsIncs, targetName):
    if not env.get('MSVS'):
        env['MSVS']['PROJECTSUFFIX'] = '.vcxproj'
        env['MSVS']['SOLUTIONSUFFIX'] = '.sln'

    msvs_variants = ['debug|x64']
    msvs_targets = [targetName]

    msvs = env.MSVSProject(
        target = 'Soulslism.vcxproj',
        srcs = vsSrcs,
        incs = vsIncs, #["godot_headers", "include", "include/gen", "include/core"],
        runfile=msvs_targets,
        buildTarget=msvs_targets,
        auto_build_solution=1,
        variant=msvs_variants)

    # Default(msvs)

def AddToVSProject(env, sources):
    for x in sources:
        if type(x) == type(""):
            fname = env.File(x).path
        else:
            fname = env.File(x)[0].path
        pieces = fname.split(".")

        if len(pieces) > 0:
            basename = pieces[0]
            basename = basename.replace('\\\\', '/')
            if os.path.isfile(basename + ".h"):
                env.vsIncs = env.vsIncs + [basename + ".h"]
            elif os.path.isfile(basename + ".hpp"):
                env.vsIncs = env.vsIncs + [basename + ".hpp"]
            if os.path.isfile(basename + ".c"):
                env.vsSrcs = env.vsSrcs + [basename + ".c"]
            elif os.path.isfile(basename + ".cpp"):
                env.vsSrcs = env.vsSrcs + [basename + ".cpp"]

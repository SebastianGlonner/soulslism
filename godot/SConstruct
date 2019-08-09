#!python
import os, subprocess


def add_sources(sources, dir, extension):
  for f in os.listdir(dir):
      if f.endswith('.' + extension):
          sources.append(dir + '/' + f)

opts = Variables([], ARGUMENTS)

# Gets the standard flags CC, CCX, etc.
env = DefaultEnvironment()

# Define our options
opts.Add(EnumVariable('target', "Compilation target", 'debug', ['d', 'debug', 'r', 'release']))
opts.Add(EnumVariable('platform', "Compilation platform", '', ['', 'windows', 'x11', 'linux', 'osx']))
opts.Add(EnumVariable('p', "Compilation target, alias for 'platform'", '', ['', 'windows', 'x11', 'linux', 'osx']))
opts.Add(BoolVariable('use_llvm', "Use the LLVM / Clang compiler", 'no'))
opts.Add(PathVariable('target_path', 'The path where the lib is installed.', 'demo/bin/'))
opts.Add(PathVariable('target_name', 'The library name.', 'libgdexample', PathVariable.PathAccept))

# Local dependency paths, adapt them to your setup
godot_headers_path = "godot-cpp/godot_headers/"
cpp_bindings_path = "godot-cpp/"
cpp_library = "libgodot-cpp"

# only support 64 at this time..
bits = 64

# Updates the environment with the option variables.
opts.Update(env)

# Process some arguments
if env['use_llvm']:
    env['CC'] = 'clang'
    env['CXX'] = 'clang++'

if env['p'] != '':
    env['platform'] = env['p']

if env['platform'] == '':
    print("No valid target platform selected.")
    quit();

# Check our platform specifics
if env['platform'] == "osx":
    env['target_path'] += 'osx/'
    cpp_library += '.osx'
    if env['target'] in ('debug', 'd'):
        env.Append(CCFLAGS = ['-g','-O2', '-arch', 'x86_64', '-std=c++17'])
        env.Append(LINKFLAGS = ['-arch', 'x86_64'])
    else:
        env.Append(CCFLAGS = ['-g','-O3', '-arch', 'x86_64', '-std=c++17'])
        env.Append(LINKFLAGS = ['-arch', 'x86_64'])

elif env['platform'] in ('x11', 'linux'):
    env['target_path'] += 'x11/'
    cpp_library += '.linux'
    if env['target'] in ('debug', 'd'):
        env.Append(CCFLAGS = ['-fPIC', '-g3','-Og', '-std=c++17'])
    else:
        env.Append(CCFLAGS = ['-fPIC', '-g','-O3', '-std=c++17'])

elif env['platform'] == "windows":
    env['target_path'] += 'win64/'
    cpp_library += '.windows'
    # This makes sure to keep the session environment variables on windows,
    # that way you can run scons in a vs 2017 prompt and it will find all the required tools
    env.Append(ENV = os.environ)

    env.Append(CCFLAGS = ['-DWIN32', '-D_WIN32', '-D_WINDOWS', '-W3', '-GR', '-D_CRT_SECURE_NO_WARNINGS'])
    if env['target'] in ('debug', 'd'):
        env.Append(CCFLAGS = ['-EHsc', '-D_DEBUG', '-MDd'])
    else:
        env.Append(CCFLAGS = ['-O2', '-EHsc', '-DNDEBUG', '-MD'])

if env['target'] in ('debug', 'd'):
    cpp_library += '.debug'
else:
    cpp_library += '.release'

cpp_library += '.' + str(bits)

# make sure our binding library is properly includes
env.Append(CPPPATH=['.', godot_headers_path, cpp_bindings_path + 'include/', cpp_bindings_path + 'include/core/', cpp_bindings_path + 'include/gen/'])
env.Append(LIBPATH=[cpp_bindings_path + 'bin/'])
env.Append(LIBS=[cpp_library])

env['PDB'] = target=env['target_path'] + env['target_name'] + '.pdb';
env['CCPDBFLAGS'] = '/Zi /Fd${TARGET}.pdb'

# tweak this if you want to use different folders, or more folders, to store your source code in.
env.Append(CPPPATH=['src/'])
sources = Glob('src/*.cpp')

library = env.SharedLibrary(target=env['target_path'] + env['target_name'], source=sources)

msvcSources = [];
msvcIncs = [];
add_sources(msvcSources, 'src', 'cpp')
add_sources(msvcIncs, 'src', 'h')

def find_visual_c_batch_file(env):
    from SCons.Tool.MSCommon.vc import get_default_version, get_host_target, find_batch_file

    version = get_default_version(env)
    (host_platform, target_platform, _) = get_host_target(env)
    return find_batch_file(env, version, host_platform, target_platform)[0]

def build_commandline(commands):
    batch_file = find_visual_c_batch_file(env)
    common_build_prefix = ['cmd /V /C set "plat=$(PlatformTarget)"',
                            '(if "$(PlatformTarget)"=="x64" (set "plat=x86_amd64"))',
                            'set "tools=yes"',
                            '(if "$(Configuration)"=="release" (set "tools=no"))',
                            'call "' + batch_file + '" !plat!']

    result = " ^& ".join(common_build_prefix + [commands])
    return result

num_jobs = 1

# windows allows us to have spaces in paths, so we need
# to double quote off the directory. However, the path ends
# in a backslash, so we need to remove this, lest it escape the
# last double quote off, confusing MSBuild
env['MSVSBUILDCOM'] = build_commandline('scons --directory="$(ProjectDir.TrimEnd(\'\\\'))" platform=windows progress=no target=$(Configuration) tools=!tools! -j' + str(num_jobs))
env['MSVSREBUILDCOM'] = build_commandline('scons --directory="$(ProjectDir.TrimEnd(\'\\\'))" platform=windows progress=no target=$(Configuration) tools=!tools! vsproj=yes -j' + str(num_jobs))
env['MSVSCLEANCOM'] = build_commandline('scons --directory="$(ProjectDir.TrimEnd(\'\\\'))" --clean platform=windows progress=no target=$(Configuration) tools=!tools! -j' + str(num_jobs))


debug_variants = ['debug|x64']
release_variants = ['release|x64']
release_debug_variants = ['release_debug|x64']
variants = debug_variants + release_variants + release_debug_variants

targetFileName = env['target_path'] + env['target_name'] + '.dll'

debug_targets = [targetFileName]
release_targets = [targetFileName]
release_debug_targets = [targetFileName]
targets = debug_targets + release_targets + release_debug_targets

msvc = env.MSVSProject(target = ['Bar' + env['MSVSPROJECTSUFFIX']],
    srcs = msvcSources,
    incs = msvcIncs,
    runfile=targets,
    buildtarget=targets,
    auto_build_solutions=1,
    variant=variants)

Default(msvc)
Default(library)

# Generates help for the -h scons option.
Help(opts.GenerateHelpText(env))

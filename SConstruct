#!python
import os, subprocess, sys



def add_sources(sources, dir, extension):
  for f in os.listdir(dir):
      if f.endswith('.' + extension):
          sources.append(dir + '/' + f)

opts = Variables([], ARGUMENTS)

# Define our options
opts.Add(EnumVariable('bits', "Target platform bits", 'default', ('default', '32', '64')))
opts.Add(EnumVariable('target', "Compilation target", 'debug', ['d', 'debug', 'r', 'release']))
opts.Add(EnumVariable('platform', "Compilation platform", 'windows', ['', 'windows', 'x11', 'linux', 'osx']))
opts.Add(BoolVariable('use_llvm', "Use the LLVM / Clang compiler", 'no'))
opts.Add(PathVariable('target_path', 'The path where the lib is installed.', 'godot/scripts/navagent/bin/'))
opts.Add(PathVariable('target_name', 'The library name.', 'navagent', PathVariable.PathAccept))
opts.Add(BoolVariable('use_lto', 'Use link-time optimization', False))
opts.Add(('target_win_version', 'Targeted Windows version, >= 0x0601 (Windows 7)', '0x0601')),

useMingw = False;

custom_tools = ['default']
if ( useMingw ):
    custom_tools = ['mingw']

# Gets the standard flags CC, CCX, etc.
env = Environment(tools=custom_tools)

env.use_mingw = useMingw;
env['use_mingw'] = useMingw;

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
    cpp_library += '.windows'

    import methods
    methods.configure_for_windows(env)

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

env.Append(CPPPATH=[
    'vendor'
])

# print env.Dump();
# sys.exit();

library = env.SharedLibrary(target=env['target_path'] + env['target_name'], source=sources)
Default(library)

# env_tests = env.Clone();

# env_tests.Append(CPPPATH=[
#     'vendor'
# ])
# testSources = Glob('tests/*.cpp')
# tests = env_tests.Program(target='test', source=testSources, LIBS=library)

# Default(tests)

# Generates help for the -h scons option.
Help(opts.GenerateHelpText(env))

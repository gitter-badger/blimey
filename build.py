#!/usr/bin/python

import os, shutil, subprocess, sys


# If on windows you must have the following in your PATH in both the user and system variables:
# C:\python27\;C:\Program Files (x86)\Mono\bin
# Also build with Windows Powershell, not the Command Prompt.

print "OS:" + sys.platform

v_build_platform_api_monomac_app = False
v_build_platform_api_xamarin_ios_app = False
v_build_platform_api_opentk_game = True
v_build_platform_api_wpf_xna4_control = False
v_build_platform_api_wpf_opentk_control = False
v_build_platform_api_xna4_game = False

c_osx_xamarin_monomac_path = '/Applications/Xamarin Studio.app/Contents/Resources/lib/MonoDevelop/AddIns/MonoDevelop.MonoMac/'
c_osx_xamarin_ios_path = '/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/2.1/'

if sys.platform == 'darwin':
  if os.path.exists (c_osx_xamarin_monomac_path) == False:
    raise Exception('The MonoMac SDK must be installed.  Expected to find it here:\n' + c_osx_xamarin_monomac_path)
  elif os.path.exists (c_osx_xamarin_ios_path) == False:
    raise Exception('The Xamarin iOS SDK must be installed.  Expected to find it here:\n' + c_osx_xamarin_ios_path)
  else:
    v_build_platform_api_monomac_app = True
    v_build_platform_api_xamarin_ios_app = True


class bcolors:
    if sys.platform == 'win32':
      HEADER = ''
      OKBLUE = ''
      OKGREEN = ''
      WARNING = ''
      FAIL = ''
      ENDC = ''
      BOLD = ''
      UNDERLINE = ''
    else:
      HEADER = '\033[95m'
      OKBLUE = '\033[94m'
      OKGREEN = '\033[92m'
      WARNING = '\033[93m'
      FAIL = '\033[91m'
      ENDC = '\033[0m'
      BOLD = '\033[1m'
      UNDERLINE = '\033[4m'

class Project:
  pass

# BUILD ALL CONTRIB LIBS
################################################################################
abacus = Project ()
abacus.out = 'abacus'
abacus.target = 'library'
abacus.path = 'source/contrib/abacus/src/main/cs/'
abacus.defines = []
abacus.references = []
abacus.additional_references = []
abacus.additional_search_paths = []
abacus.additional_sources = []

oats = Project ()
oats.out = 'oats'
oats.target = 'library'
oats.path = 'source/contrib/oats/src/main/cs/'
oats.defines = []
oats.references = []
oats.additional_references = []
oats.additional_search_paths = []
oats.additional_sources = []

zlib = Project ()
zlib.out = 'ZLIB'
zlib.target = 'library'
zlib.path = 'source/contrib/zlib/src/main/cs/'
zlib.defines = ['NET45']
zlib.references = []
zlib.additional_references = []
zlib.additional_search_paths = []
zlib.additional_sources = []

pngcs = Project ()
pngcs.out = 'PNGcs'
pngcs.target = 'library'
pngcs.path = 'source/contrib/pngcs/src/main/cs/'
pngcs.defines = []
pngcs.references = [zlib]
pngcs.additional_references = []
pngcs.additional_search_paths = []
pngcs.additional_sources = []

fastjson = Project ()
fastjson.out = 'fastJSON'
fastjson.target = 'library'
fastjson.path = 'source/contrib/fastjson/src/main/cs/'
fastjson.defines = []
fastjson.references = []
fastjson.additional_references = ['System.Data']
fastjson.additional_search_paths = []
fastjson.additional_sources = []

sstext = Project ()
sstext.out = 'sstext'
sstext.target = 'library'
sstext.path = 'source/contrib/servicestack.text/src/main/cs/'
sstext.defines = []
sstext.references = []
sstext.additional_references = ['System.Runtime.Serialization', 'System.Data', 'System.Data.Linq', 'System.Configuration']
sstext.additional_search_paths = []
sstext.additional_sources = []

ndoptions = Project ()
ndoptions.out = 'ndoptions'
ndoptions.target = 'library'
ndoptions.path = 'source/contrib/ndesk.options/src/main/cs/'
ndoptions.defines = []
ndoptions.references = []
ndoptions.additional_references = ['System.Runtime.Serialization', 'System.Data', 'System.Data.Linq', 'System.Configuration']
ndoptions.additional_search_paths = []
ndoptions.additional_sources = []


# Platform Core
################################################################################

blimey_platform_foundation = Project ()
blimey_platform_foundation.out = 'blimey.platform.foundation'
blimey_platform_foundation.target = 'library'
blimey_platform_foundation.path = 'source/blimey.platform.foundation/src/main/cs/'
blimey_platform_foundation.defines = []
blimey_platform_foundation.references = []
blimey_platform_foundation.additional_references = []
blimey_platform_foundation.additional_search_paths = []
blimey_platform_foundation.additional_sources = []

blimey_platform_packed = Project ()
blimey_platform_packed.out = 'blimey.platform.packed'
blimey_platform_packed.target = 'library'
blimey_platform_packed.path = 'source/blimey.platform.packed/src/main/cs/'
blimey_platform_packed.defines = []
blimey_platform_packed.references = [abacus]
blimey_platform_packed.additional_references = []
blimey_platform_packed.additional_search_paths = []
blimey_platform_packed.additional_sources = []

blimey_platform_util = Project ()
blimey_platform_util.out = 'blimey.platform.util'
blimey_platform_util.target = 'library'
blimey_platform_util.path = 'source/blimey.platform.util/src/main/cs/'
blimey_platform_util.defines = []
blimey_platform_util.references = [blimey_platform_packed, abacus]
blimey_platform_util.additional_references = []
blimey_platform_util.additional_search_paths = []
blimey_platform_util.additional_sources = []

blimey_platform_model = Project ()
blimey_platform_model.out = 'blimey.platform.model'
blimey_platform_model.target = 'library'
blimey_platform_model.path = 'source/blimey.platform.model/src/main/cs/'
blimey_platform_model.defines = []
blimey_platform_model.references = [abacus, blimey_platform_packed, blimey_platform_util]
blimey_platform_model.additional_references = []
blimey_platform_model.additional_search_paths = []
blimey_platform_model.additional_sources = []

blimey_platform_api = Project ()
blimey_platform_api.out = 'blimey.platform.api'
blimey_platform_api.target = 'library'
blimey_platform_api.path = 'source/blimey.platform.api/src/main/cs/'
blimey_platform_api.defines = []
blimey_platform_api.references = [abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model]
blimey_platform_api.additional_references = []
blimey_platform_api.additional_search_paths = []
blimey_platform_api.additional_sources = []


blimey_platform_logging = Project ()
blimey_platform_logging.out = 'blimey.platform.logging'
blimey_platform_logging.target = 'library'
blimey_platform_logging.path = 'source/blimey.platform.logging/src/main/cs/'
blimey_platform_logging.defines = []
blimey_platform_logging.references = [abacus, blimey_platform_packed, blimey_platform_util, blimey_platform_foundation, blimey_platform_model, blimey_platform_api]
blimey_platform_logging.additional_references = []
blimey_platform_logging.additional_search_paths = []
blimey_platform_logging.additional_sources = []

blimey_platform = Project ()
blimey_platform.out = 'blimey.platform'
blimey_platform.target = 'library'
blimey_platform.path = 'source/blimey.platform/src/main/cs/'
blimey_platform.defines = []
blimey_platform.references = [abacus, blimey_platform_packed, blimey_platform_util, blimey_platform_foundation, blimey_platform_model, blimey_platform_api, blimey_platform_logging]
blimey_platform.additional_references = []
blimey_platform.additional_search_paths = []
blimey_platform.additional_sources = []


# Platforms
################################################################################


if v_build_platform_api_monomac_app:
  blimey_platform_api_monomac_app = Project ()
  blimey_platform_api_monomac_app.out = 'blimey.platform.api.monomac-app'
  blimey_platform_api_monomac_app.target = 'library'
  blimey_platform_api_monomac_app.path = 'source/blimey.platform.api.monomac-app/src/main/cs/'
  blimey_platform_api_monomac_app.defines = ['PLATFORM_API_MONOMAC_APP']
  blimey_platform_api_monomac_app.references = [abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model, blimey_platform_api, blimey_platform_logging]
  blimey_platform_api_monomac_app.additional_references = ['System.Drawing', 'MonoMac']
  blimey_platform_api_monomac_app.additional_search_paths = [c_osx_xamarin_monomac_path]
  blimey_platform_api_monomac_app.additional_sources = ['source/shared/CommonOpenTK.cs']

if v_build_platform_api_xamarin_ios_app:
  blimey_platform_api_xamarin_ios_app = Project ()
  blimey_platform_api_xamarin_ios_app.out = 'blimey.platform.api.xamarin-ios-app'
  blimey_platform_api_xamarin_ios_app.target = 'library'
  blimey_platform_api_xamarin_ios_app.path = 'source/blimey.platform.api.xamarin-ios-app/src/main/cs/'
  blimey_platform_api_xamarin_ios_app.defines = ['PLATFORM_API_XAMARIN_IOS_APP']
  blimey_platform_api_xamarin_ios_app.references = [abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model, blimey_platform_api, blimey_platform_logging]
  blimey_platform_api_xamarin_ios_app.additional_references = ['OpenTK-1.0', 'monotouch']
  blimey_platform_api_xamarin_ios_app.additional_search_paths = [c_osx_xamarin_ios_path]
  blimey_platform_api_xamarin_ios_app.additional_sources = ['source/shared/CommonOpenTK.cs']

if v_build_platform_api_opentk_game:
  blimey_platform_api_opentk_game = Project ()
  blimey_platform_api_opentk_game.out = 'blimey.platform.api.opentk-game'
  blimey_platform_api_opentk_game.target = 'library'
  blimey_platform_api_opentk_game.path = 'source/blimey.platform.api.opentk-game/src/main/cs/'
  blimey_platform_api_opentk_game.defines = ['PLATFORM_API_OPENTK_GAME']
  blimey_platform_api_opentk_game.references = [abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model, blimey_platform_api, blimey_platform_logging]
  blimey_platform_api_opentk_game.additional_references = ['System.Drawing', 'OpenTK']
  blimey_platform_api_opentk_game.additional_search_paths = ['packages/']
  blimey_platform_api_opentk_game.additional_sources = ['source/shared/CommonOpenTK.cs']


# Blimey Asset System
################################################################################

blimey_asset_model = Project ()
blimey_asset_model.out = 'blimey.asset.model'
blimey_asset_model.target = 'library'
blimey_asset_model.path = 'source/blimey.asset.model/src/main/cs/'
blimey_asset_model.defines = []
blimey_asset_model.references = []
blimey_asset_model.additional_references = []
blimey_asset_model.additional_search_paths = []
blimey_asset_model.additional_sources = []

blimey_asset_buildpipeline = Project ()
blimey_asset_buildpipeline.out = 'blimey.asset.buildpipeline'
blimey_asset_buildpipeline.target = 'library'
blimey_asset_buildpipeline.path = 'source/blimey.asset.buildpipeline/src/main/cs/'
blimey_asset_buildpipeline.defines = []
blimey_asset_buildpipeline.references = [blimey_asset_model]
blimey_asset_buildpipeline.additional_references = []
blimey_asset_buildpipeline.additional_search_paths = []
blimey_asset_buildpipeline.additional_sources = []

blimey_asset_builder = Project ()
blimey_asset_builder.out = 'blimey.asset.builder'
blimey_asset_builder.target = 'exe'
blimey_asset_builder.path = 'source/blimey.asset.builder/src/main/cs/'
blimey_asset_builder.defines = []
blimey_asset_builder.references = [blimey_asset_model, blimey_asset_buildpipeline, oats, sstext, ndoptions]
blimey_asset_builder.additional_references = []
blimey_asset_builder.additional_search_paths = []
blimey_asset_builder.additional_sources = []



# Blimey Engine
################################################################################

blimey_engine_model = Project ()
blimey_engine_model.out = 'blimey.engine.model'
blimey_engine_model.target = 'library'
blimey_engine_model.path = 'source/blimey.engine.model/src/main/cs/'
blimey_engine_model.defines = []
blimey_engine_model.references = [abacus, oats, blimey_platform_packed, blimey_platform_model, blimey_asset_model]
blimey_engine_model.additional_references = []
blimey_engine_model.additional_search_paths = []
blimey_engine_model.additional_sources = []

blimey_engine_assets_buildpipelines = Project ()
blimey_engine_assets_buildpipelines.out = 'blimey.engine.assets.buildpipelines'
blimey_engine_assets_buildpipelines.target = 'library'
blimey_engine_assets_buildpipelines.path = 'source/blimey.engine.assets.buildpipelines/src/main/cs/'
blimey_engine_assets_buildpipelines.defines = []
blimey_engine_assets_buildpipelines.references = [
  abacus, blimey_platform_packed, blimey_platform_model, blimey_asset_model,
  blimey_engine_model, oats, zlib, pngcs,
  blimey_asset_buildpipeline, sstext]
blimey_engine_assets_buildpipelines.additional_references = []
blimey_engine_assets_buildpipelines.additional_search_paths = []
blimey_engine_assets_buildpipelines.additional_sources = []

blimey_engine = Project ()
blimey_engine.out = 'blimey.engine'
blimey_engine.target = 'library'
blimey_engine.path = 'source/blimey.engine/src/main/cs/'
blimey_engine.defines = []
blimey_engine.references = [
  abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model,
  blimey_platform, blimey_asset_model, blimey_engine_model, oats, blimey_platform_util, blimey_platform_logging]
blimey_engine.additional_references = []
blimey_engine.additional_search_paths = []
blimey_engine.additional_sources = []




# Blimey Demo Projects
################################################################################


blimey_platform_demo = Project ()
blimey_platform_demo.out = 'blimey.platform.demo'
blimey_platform_demo.target = 'library'
blimey_platform_demo.path = 'examples/platform-demo/blimey.platform.demo/src/main/cs/'
blimey_platform_demo.defines = []
blimey_platform_demo.references = [
  abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model,
  blimey_platform]
blimey_platform_demo.additional_references = []
blimey_platform_demo.additional_search_paths = []
blimey_platform_demo.additional_sources = []

blimey_engine_demo = Project ()
blimey_engine_demo.out = 'blimey.engine.demo'
blimey_engine_demo.target = 'library'
blimey_engine_demo.path = 'examples/engine-demo/blimey.engine.demo/src/main/cs/'
blimey_engine_demo.defines = []
blimey_engine_demo.references = [
  abacus, blimey_platform_packed, blimey_platform_foundation, blimey_platform_model,
  blimey_platform, blimey_asset_model, blimey_engine_model, oats, blimey_engine]
blimey_engine_demo.additional_references = []
blimey_engine_demo.additional_search_paths = []
blimey_engine_demo.additional_sources = []


################################################################################


projects = [
  # Contrib libs, no dependencies (except for pngcs depending on zlib).
  abacus,
  oats,
  zlib,
  pngcs,
  fastjson,
  sstext,
  ndoptions,

  # The blimey asset system libaries, no depencies on anything other than contrib projects.
  blimey_asset_model,
  blimey_asset_buildpipeline,


  # Blimey asset system build tool, no depencies on anything other than contrib projects.
  # This tool is interesting, because although it depends on very little at compile time
  # at run time it dynamically loads assemblies specified on the command line that
  # contain build pipelines.
  # When used with Blimey engine the tool should always be provided the location
  # of `blimey.engine.assets.buildpipelines.dll` and `blimey.platform.model.dll`
  # via the commandline.
  blimey_asset_builder,


  # The low level Blimey platform, no depencies on anything other than contrib projects.
  # The plaform layer doesn't know about the blimey asset system.
  blimey_platform_foundation,
  blimey_platform_packed,
  blimey_platform_util,
  blimey_platform_model,
  blimey_platform_api,
  blimey_platform_logging,
  blimey_platform,


  # The Blimey engine, depends on everything.
  blimey_engine_model,
  blimey_engine,

  # Nothing depends on this at compile time, it contains pipelines for the asset
  # types found in `blimey.engine.model.dll`, both the location of this dll, and
  # the dll aforementioned should be provided as commandline arguments to the
  # Blimey asset build tool.
  blimey_engine_assets_buildpipelines,

  # Demos
  blimey_platform_demo,
  blimey_engine_demo
  ]


if v_build_platform_api_opentk_game:
  projects.append (blimey_platform_api_opentk_game)

if v_build_platform_api_monomac_app:
  projects.append (blimey_platform_api_monomac_app)

if v_build_platform_api_xamarin_ios_app:
  projects.append (blimey_platform_api_xamarin_ios_app)

################################################################################


try:
  shutil.rmtree('bin')
except:
  pass

os.mkdir ('bin')

fail_count = 0

for project in projects:
  print ''

  if project.target == 'exe':
    _output = project.out + '.exe'
  else:
    _output = project.out + '.dll'

  print 'COMPILING: '+ _output

  _out = '-out:bin/' + _output
  _target = '-target:' + project.target
  _recurse = '-recurse:' + project.path + '*.cs'

  _lib = ['-lib:bin/']
  _lib.extend (map (lambda x: '-lib:' + x, project.additional_search_paths))
  _define = []
  _reference = []

  if len (project.defines) > 0:
    _define = map (lambda x: '-define:' + x, project.defines)
  if len (project.references) > 0:
    _reference.extend (map (lambda x: '-reference:' + x.out + '.dll', project.references))
  if len (project.additional_references) > 0:
    _reference.extend (map (lambda x: '-reference:' + x + '.dll', project.additional_references))

  if sys.platform == 'win32':
    _cmd = ['mcs.bat']
  else:
    _cmd = ['mcs']

  _cmd.append('-unsafe')
  #_cmd.append('-debug')
  _cmd.append('-define:DEBUG')
  _cmd.append(_out)
  _cmd.append(_target)
  _cmd.append(_recurse)
  _cmd.extend(map (lambda x: '-recurse:' + x, project.additional_sources))
  _cmd.extend(_lib)
  _cmd.extend(_define)
  _cmd.extend(_reference)

  print " ".join (_cmd )

  _ret = subprocess.call (_cmd)

  if _ret == 0:
    print bcolors.OKGREEN + 'SUCCESS' + bcolors.ENDC
  else:
    print bcolors.FAIL + 'FAILURE' + bcolors.ENDC
    fail_count = fail_count + 1

print ''
print '--------------------------------------'
print ''

if fail_count == 0:
  print bcolors.OKGREEN + 'BUILD SUCCEEDED' + bcolors.ENDC
else:
  print bcolors.FAIL + 'BUILD FAILED: ' + fail_count + '/' + len(projects) + bcolors.ENDC

sys.exit (fail_count)
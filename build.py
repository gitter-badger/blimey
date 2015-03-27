#!/usr/bin/python

import os, shutil

class Project:
  pass

# BUILD ALL CONTRIB LIBS
################################################################################
abacus = Project ()
abacus.out = 'Abacus'
abacus.target = 'library'
abacus.path = 'source/contrib/abacus/src/main/cs/'
abacus.defines = []
abacus.references = []

oats = Project ()
oats.out = 'Oats'
oats.target = 'library'
oats.path = 'source/contrib/oats/src/main/cs/'
oats.defines = []
oats.references = []

zlib = Project ()
zlib.out = 'ZLIB'
zlib.target = 'library'
zlib.path = 'source/contrib/zlib/src/main/cs/'
zlib.defines = ['NET45']
zlib.references = []

pngcs = Project ()
pngcs.out = 'PNGcs'
pngcs.target = 'library'
pngcs.path = 'source/contrib/pngcs/src/main/cs/'
pngcs.defines = []
pngcs.references = [zlib]

# BUILD ALL BLIMEY LIBS
################################################################################

blimey_foundation = Project ()
blimey_foundation.out = 'Blimey.Foundation'
blimey_foundation.target = 'library'
blimey_foundation.path = 'source/blimey.foundation/src/main/cs/'
blimey_foundation.defines = []
blimey_foundation.references = []

blimey_platform_packed = Project ()
blimey_platform_packed.out = 'Blimey.Platform.Packed'
blimey_platform_packed.target = 'library'
blimey_platform_packed.path = 'source/blimey.platform.packed/src/main/cs/'
blimey_platform_packed.defines = []
blimey_platform_packed.references = [abacus]

blimey_platform_model = Project ()
blimey_platform_model.out = 'Blimey.Platform.Model'
blimey_platform_model.target = 'library'
blimey_platform_model.path = 'source/blimey.platform.model/src/main/cs/'
blimey_platform_model.defines = []
blimey_platform_model.references = [abacus, blimey_platform_packed]

blimey_platform_api = Project ()
blimey_platform_api.out = 'Blimey.Platform.Api'
blimey_platform_api.target = 'library'
blimey_platform_api.path = 'source/blimey.platform.api/src/main/cs/'
blimey_platform_api.defines = []
blimey_platform_api.references = [abacus, blimey_platform_packed, blimey_foundation, blimey_platform_model]

blimey_platform_wrapper = Project ()
blimey_platform_wrapper.out = 'Blimey.Platform.Wrapper'
blimey_platform_wrapper.target = 'library'
blimey_platform_wrapper.path = 'source/blimey.platform.wrapper/src/main/cs/'
blimey_platform_wrapper.defines = []
blimey_platform_wrapper.references = [abacus, blimey_platform_packed, blimey_foundation, blimey_platform_model, blimey_platform_api]




blimey_asset_model = Project ()
blimey_asset_model.out = 'Blimey.Asset.Model'
blimey_asset_model.target = 'library'
blimey_asset_model.path = 'source/blimey.asset.model/src/main/cs/'
blimey_asset_model.defines = []
blimey_asset_model.references = []

blimey_asset_buildpipeline = Project ()
blimey_asset_buildpipeline.out = 'Blimey.Asset.BuildPipeline'
blimey_asset_buildpipeline.target = 'library'
blimey_asset_buildpipeline.path = 'source/blimey.asset.buildpipeline/src/main/cs/'
blimey_asset_buildpipeline.defines = []
blimey_asset_buildpipeline.references = [blimey_asset_model]

blimey_asset_builder = Project ()
blimey_asset_builder.out = 'Blimey.Asset.Builder'
blimey_asset_builder.target = 'exe'
blimey_asset_builder.path = 'source/blimey.asset.builder/src/main/cs/'
blimey_asset_builder.defines = []
blimey_asset_builder.references = [blimey_asset_model, blimey_asset_buildpipeline, oats]




blimey_engine_model = Project ()
blimey_engine_model.out = 'Blimey.Engine.Model'
blimey_engine_model.target = 'library'
blimey_engine_model.path = 'source/blimey.engine.model/src/main/cs/'
blimey_engine_model.defines = []
blimey_engine_model.references = [abacus, blimey_platform_packed, blimey_platform_model]

blimey_engine_assets = Project ()
blimey_engine_assets.out = 'Blimey.Engine.Assets'
blimey_engine_assets.target = 'library'
blimey_engine_assets.path = 'source/blimey.engine.assets/src/main/cs/'
blimey_engine_assets.defines = []
blimey_engine_assets.references = [abacus, blimey_platform_packed, blimey_platform_model, blimey_asset_model, blimey_engine_model]

blimey_engine_assets_buildpipelines = Project ()
blimey_engine_assets_buildpipelines.out = 'Blimey.Engine.Assets.BuildPipelines'
blimey_engine_assets_buildpipelines.target = 'library'
blimey_engine_assets_buildpipelines.path = 'source/blimey.engine.assets.buildpipelines/src/main/cs/'
blimey_engine_assets_buildpipelines.defines = []
blimey_engine_assets_buildpipelines.references = [
  abacus, blimey_platform_packed, blimey_platform_model, blimey_asset_model,
  blimey_engine_model, blimey_engine_assets, oats, zlib, pngcs,
  blimey_asset_buildpipeline]

blimey_engine_serialisation = Project ()
blimey_engine_serialisation.out = 'Blimey.Engine.Serialisation'
blimey_engine_serialisation.target = 'library'
blimey_engine_serialisation.path = 'source/blimey.engine.serialisation/src/main/cs/'
blimey_engine_serialisation.defines = []
blimey_engine_serialisation.references = [
  abacus, blimey_platform_packed, blimey_platform_model, blimey_engine_model,
  blimey_asset_model, oats, blimey_engine_assets]

blimey_engine = Project ()
blimey_engine.out = 'Blimey.Engine'
blimey_engine.target = 'library'
blimey_engine.path = 'source/blimey.engine/src/main/cs/'
blimey_engine.defines = []
blimey_engine.references = [
  abacus, blimey_platform_packed, blimey_foundation, blimey_platform_model,
  blimey_platform_wrapper, blimey_asset_model, blimey_engine_model,
  blimey_engine_serialisation, oats, blimey_engine_assets]


projects = [
  abacus,
  oats,
  zlib,
  pngcs,
  blimey_foundation,
  blimey_platform_packed,
  blimey_platform_model,
  blimey_platform_api,
  blimey_platform_wrapper,
  blimey_asset_model,
  blimey_asset_buildpipeline,
  blimey_asset_builder,
  blimey_engine_model,
  blimey_engine_assets,
  blimey_engine_serialisation,
  blimey_engine,
  blimey_engine_assets_buildpipelines]

shutil.rmtree('bin')
os.mkdir ('bin')

for project in projects:
  print 'Building '+ project.out
  _out = '-out:bin/' + project.out + '.dll'
  _target = '-target:' + project.target
  _recurse = '-recurse:' + project.path + '*.cs'
  _define = ''
  _reference = ''
  _lib = '-lib:bin/'
  if len (project.defines) > 0:
    _define = ' '.join (map (lambda x: '-define:' + x, project.defines))
  if len (project.references) > 0:
    _reference = ' '.join (map (lambda x: '-reference:' + x.out + '.dll', project.references))
  _cmd = 'mcs -unsafe ' + ' '.join ([_out, _lib, _target, _recurse, _define, _reference])

  print _cmd
  os.system(_cmd)
  print 'COMPLETE'
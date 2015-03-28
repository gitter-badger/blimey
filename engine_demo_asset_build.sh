#!/bin/bash

mono --debug bin/blimey.asset.builder.exe --assembly=bin/blimey.engine.assets.buildpipelines.dll --assembly=bin/blimey.engine.model.dll --directory=examples/engine-demo/blimey.engine.demo/

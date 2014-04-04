## Cor Asset Pipeline

Artists and tech artists produce raw assets like .png, .obj, .mat, .fsh, vsh, .fx; each one of these files is refered to as a source.  Some sources are only useful when combined with other sources, for example a .mat file and a .obj file go hand in hand.

A collection of sources that go together are called a source-set and are defined by and asset-definition file.

    + source + source + source + {asset-definition-file}
    = source-set

Source-sets can be imported by a resource-builder which converts the source-set into an intermediate format, a resource.

    + source-set => [resource-builder]
    = resource

Resources are not used by the by the Cor runtime engine, instead they are processed by an asset-builder which coverts the resource into the final asset format.

    + resource => [asset-builder]
    = asset

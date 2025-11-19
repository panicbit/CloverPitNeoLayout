#!/usr/bin/env nu

def main [] {
    let mod_name = "CloverPitNeoLayout"
    let target = "netstandard2.1"
    let configuration = "Release"
    let build_dir = $"bin/($configuration)"
    let release_dir = $"bin/($mod_name)"
    
    rm -rf $build_dir
    rm -rf $release_dir

    dotnet build -c $configuration

    cp -r $"($build_dir)/($target)/" $release_dir

    cd bin
    ^zip -r $"($mod_name).zip" $mod_name
}

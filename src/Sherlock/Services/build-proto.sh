#!/usr/bin/env bash
/users/andrea/.nuget/packages/google.protobuf.tools/3.5.1/tools/macosx_x64/protoc -I=/users/andrea/.nuget/packages/google.protobuf.tools/3.5.1/tools -I=. --csharp_opt=file_extension=.g.cs --csharp_out=. --grpc_out . --plugin=protoc-gen-grpc=/users/andrea/.nuget/packages/Grpc.Tools/1.8.3/tools/macosx_x64/grpc_csharp_plugin Sherlock.proto

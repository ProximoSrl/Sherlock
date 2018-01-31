#!/usr/bin/env bash
/users/andrea/.nuget/packages/google.protobuf.tools/3.4.0/tools/macosx_x64/protoc -I=/users/andrea/.nuget/packages/google.protobuf.tools/3.4.0/tools -I=. -I=../Services --csharp_opt=file_extension=.g.cs --csharp_out=. Messages.proto

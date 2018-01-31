%USERPROFILE%\.nuget\packages\google.protobuf.tools\3.5.1\tools\windows_x64\protoc.exe -I=%USERPROFILE%\.nuget\packages\google.protobuf.tools\3.5.1\tools -I=. --csharp_opt=file_extension=.g.cs --csharp_out=. --grpc_out . --plugin=protoc-gen-grpc=%userprofile%\.nuget\packages\Grpc.Tools\1.8.3\tools\windows_x64\grpc_csharp_plugin.exe Sherlock.proto

pause
exit
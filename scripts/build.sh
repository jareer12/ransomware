## Initialize
mkdir ./build

## Clear Build Folder
rm ./build/*

## Compile Encryptor
cd ./src/encryptor/ && dotnet publish -r win10-x64 -p:PublishSingleFile=true -c Release --self-contained
cd -
mv ./src/encryptor/bin/Release/net6.0/win10-x64/publish/*.exe ./build/

## Compile Decryptor
cd ./src/decryptor/ && dotnet publish -r win10-x64 -p:PublishSingleFile=true -c Release --self-contained
cd -
mv ./src/decryptor/bin/Release/net6.0/win10-x64/publish/*.exe ./build/

## Clean The Source Directory
rm -rf ./src/*/bin
rm -rf ./src/*/obj

# mcs -out:main.exe ./src/main.cs
# mcs -out:decryptor.exe ./src/decryptor.cs
# mv ./*.exe ./build

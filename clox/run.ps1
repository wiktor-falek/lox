if (-not (Test-Path -Path "build/")) {
    New-Item -Path "build/" -ItemType Directory
}

gcc main.c -o build/main.o -c
gcc chunk.c -o build/chunk.o -c
gcc memory.c -o build/memory.o -c
gcc -o clox build/main.o build/chunk.o build/memory.o
gcc build/main.o build/chunk.o build/memory.o -o build/clox.exe

./build/clox.exe

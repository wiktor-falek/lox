if (-not (Test-Path -Path "build/")) {
    New-Item -Path "build/" -ItemType Directory
}

# $files = @(
#     "main.c",
#     "chunk.c",
#     "memory.c"
#     )

# for ( $index = 0; $index -lt $files.count; $index++)
# {
#     "Item: [{0}]" -f $files[$index]
# }

gcc main.c -o build/main.o -c
gcc chunk.c -o build/chunk.o -c
gcc memory.c -o build/memory.o -c
gcc debug.c -o build/debug.o -c
gcc value.c -o build/value.o -c
gcc build/main.o build/chunk.o build/memory.o build/debug.o build/value.o -o build/clox.exe

./build/clox.exe
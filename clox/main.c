#include "chunk.h"
#include "common.h"
#include "debug.h"
#include <stdio.h>

int main(int argc, const char *argv[])
{
	Chunk chunk;
	init_chunk(&chunk);

	int constant = add_constant(&chunk, 1.2);
	write_chunk(&chunk, OP_CONSTANT);
	write_chunk(&chunk, constant);

	disassemble_chunk(&chunk, "constant chunk");
	free_chunk(&chunk);

    printf("Done\n");
	return 0;
}

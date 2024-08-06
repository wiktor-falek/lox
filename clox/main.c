#include "chunk.h"
#include "common.h"
#include "debug.h"

int main(int argc, const char *argv[])
{
	Chunk chunk;
	init_chunk(&chunk);

	write_chunk(&chunk, OP_RETURN, 0);

	int constant = add_constant(&chunk, 1.2);
	write_chunk(&chunk, OP_CONSTANT, 0);
	write_chunk(&chunk, constant, 0);

	disassemble_chunk(&chunk, "test chunk");
	free_chunk(&chunk);
	return 0;
}

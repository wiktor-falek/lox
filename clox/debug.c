#include "debug.h"
#include <stdio.h>

void disassemble_chunk(Chunk *chunk, const char *name)
{
	printf("== %s ==\n", name);

	for (int offset = 0; offset < chunk->count;) {
		offset = disassemble_instruction(chunk, offset);
	}
}

int constant_instruction(const char *name, Chunk *chunk, int offset)
{
	uint8_t constant = chunk->code[offset + 1];
	printf("%-16s %4d '\n", name, constant);
	return offset + 1;
}

int simple_instruction(const char *name, int offset)
{
	printf("%s\n", name);
	return offset + 1;
}

int disassemble_instruction(Chunk *chunk, int offset)
{
	printf("%04d ", offset);

	uint8_t instruction = chunk->code[offset];
	switch (instruction) {
	case OP_CONSTANT:
		return constant_instruction("OP_CONSTANT", chunk, offset);
	case OP_RETURN:
		return simple_instruction("OP_RETURN", offset);
	default:
		printf("Unknown opcode %d\n", instruction);
		return offset + 1;
	}
}

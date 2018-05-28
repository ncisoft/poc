#ifndef __LOG_H
#define __LOG_H

#ifdef __cplusplus
extern "C"
{
#endif
// code here ...

#include <unistd.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include <stdbool.h>
#include <string.h>
#include <assert.h>
#include <execinfo.h>

#define __C_BLACK__   "\x1b[30m" 
#define __C_RED__     "\x1b[31m" 
#define __C_GREEN__   "\x1b[32m"
#define __C_BLUE__    "\x1b[34m"
#define __C_YELLOW__  "\x1b[33m"
#define __C_MAGENTA__ "\x1b[35m"
#define __C_CYAN__    "\x1b[36m"
#define __C_WHITE__   "\x1b[37m"
#define __C_RESET__   "\x1b[0m"

#define STD_LOG_BRIEF false

#define STD_LOGLEVEL_DEBUG       7
#define STD_LOGLEVEL_INFO        6
#define STD_LOGLEVEL_WARNING     5
#define STD_LOGLEVEL_ERROR       4
#define STD_LOGLEVEL_FATAL       4

#define STD_LOG(LEVEL, logstr, color, __fmt, ...)       if (LEVEL <= __std_log_level)  {   \
  if (STD_LOG_BRIEF) \
  fprintf(stdout, color "%s:%d\t%s\t", __FILE__,  __LINE__, logstr); \
  else \
  fprintf(stdout, color "%s:%s:%d\t%s\t", __FILE__,   __FUNCTION__,__LINE__, logstr); \
  fprintf(stdout, __fmt, ##__VA_ARGS__); \
  fprintf(stdout, __C_RESET__ ); \
}

#define std_debug(__fmt, ...)     STD_LOG(STD_LOGLEVEL_DEBUG, "DEBUG", __C_RESET__,  __fmt, ##__VA_ARGS__)
#define std_info(__fmt, ...)     STD_LOG(STD_LOGLEVEL_INFO, "INFO", __C_YELLOW__,  __fmt, ##__VA_ARGS__)
#define std_warn(__fmt, ...)     STD_LOG(STD_LOGLEVEL_WARNING, "WARN", __C_MAGENTA__,  __fmt, ##__VA_ARGS__)
#define std_error(__fmt, ...)     STD_LOG(STD_LOGLEVEL_ERROR, "ERROR", __C_RED__,  __fmt, ##__VA_ARGS__)
#define std_fatal(__fmt, ...)     STD_LOG(STD_LOGLEVEL_ERROR, "FATAL", __C_YELLOW__,  __fmt, ##__VA_ARGS__)

extern int __std_log_level;

#define std_assert(expr)  { if (!(expr)) { std_print_stacktrace(__FILE__,  __LINE__); assert(expr); } } 

#define std_println() fprintf(stderr, "\n");

static inline void std_print_stacktrace(const char *fname, int lineno)
{
	int size = 16;
	void * array[16];
	int stack_num = backtrace(array, size);

	std_debug("backstrace: %s:%d ...\n", fname, lineno);

	char ** stacktrace = backtrace_symbols(array, stack_num);
	for (int i = 0; i < stack_num; ++i)
	{
		printf(__C_MAGENTA__);
		printf("\t%s\n", stacktrace[i]);
		printf(__C_RESET__);

	}
	free(stacktrace);
}


#ifdef __cplusplus
}
#endif

#endif


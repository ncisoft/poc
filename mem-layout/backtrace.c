#include <stdio.h>
#include <stdlib.h>
#include <execinfo.h>
#include <string.h>
#ifndef __GNUC__
#error "GCC was required"
#endif
#ifndef __clang__
#pragma GCC diagnostic ignored "-Wframe-address"
#endif

void  *extract_addr(char *strack_info)
{
#ifdef __linux__
  char *cp_addr = strchr(strack_info, '[');
  char *cp_offset = strchr(strack_info, '+');
  if (cp_addr && cp_offset)
    {
      void *p;
      int offset;
      int rcode = sscanf(cp_addr, "[%p]", &p);
      int rcode2 = sscanf(cp_offset, "+%x)", &offset);
      if (rcode == 1 && rcode2 == 1)
        {
          return p - offset;
        }
    }
#elif defined(__APPLE__)
  char *cp_addr=strstr(strack_info, "0x");
  char *cp_offset=strstr(strack_info, "+ ");

  if (cp_addr && cp_offset)
    {
      void *p;
      int offset;
      int rcode = sscanf(cp_addr, "%p ", &p);
      int rcode2 = sscanf(cp_offset, "+ %d", &offset);

      printf("p=%p, offset=%d\n", p, offset);
      if (rcode == 1 && rcode2 == 1)
        return p - offset;
    }
#else
#error "unsupported platform"
#endif
  return NULL;

}
void f()
{
  void *ip;
      int size = 16;
      void * array[16];
      char *cp = "[0x7fb77ea342e1]\n";
      int stack_num = backtrace(array, size);
      printf("stack_num=%d\n", stack_num);
      int rc=sscanf(cp, "[%p]", &ip);
      printf("ip=%p, rc=%d\n", ip, rc);
      char ** stacktrace = backtrace_symbols(array, stack_num);
      for (int i = 0; i < stack_num; ++i)
        {
          printf("\t[%d] %p \t %s\n", i, extract_addr(stacktrace[i]), stacktrace[i]);
        }
      free(stacktrace);


  printf("[0]=%p, [1] =  %p\n" , __builtin_frame_address(0), __builtin_frame_address(1));
#if 0
  ip = __builtin_return_address (3);
  printf("--%p\n", ip);
  for (unsigned int i=2; i <=1; i++)
    {
      ip = __builtin_frame_address (5);
      printf("%i,%p\n" , i, ip);
      if (!ip)
        break;
    }
  //printk("Caller is %pS\n", __builtin_return_address(0));
#endif
}

void g()
{
  f();
}

typedef struct
{
  int id;
  int age;
  int money;
} foo_t;

void *extract_caller_func_addr(int iLevel)
{
  void *ip_out = NULL;
  void * array[8];
  int size = sizeof(array)/sizeof(void *);
  int stack_num = backtrace(array, size);
  char ** stacktrace = backtrace_symbols(array, stack_num);
  for (int i = iLevel; i < stack_num; ++i)
    {
      printf("\t[%d] %p \t %s\n", i, extract_addr(stacktrace[i]), stacktrace[i]);
    }
  if (iLevel < stack_num)
    {
      ip_out =  extract_addr(stacktrace[iLevel]);
    }
  free(stacktrace);



  printf("__builtin_return_address[0]=%p\n" , __builtin_return_address(0));
  return ip_out;;
}

int main()
{
  foo_t foo = { .id=9, .money=8 };
  printf("main=%p g=%p f=%p\n", main, g, f);
  printf("foo.id=%d\n", foo.age);
  //g();
  void *ip=extract_caller_func_addr(1);
  printf("main=%p, extract_caller_func_addr(1)=%p, is_equals=%s\n", main, ip, (ip == &main) ? "true" : "false");
}

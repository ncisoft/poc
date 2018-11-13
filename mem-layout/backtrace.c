#include <stdio.h>
#include <execinfo.h>
#include <string.h>
#pragma GCC diagnostic ignored "-Wframe-address"

void  *extract_addr(char *strack_info)
{
  char *cp = strchr(strack_info, '+');
  if (cp)
    {
      void *p;
      int code = sscanf(cp, "+%p) ", &p);
      if (code == 1)
        return p;
    }
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
          printf("\t[%d] %p %s\n", i, extract_addr(stacktrace[i]), stacktrace[i]);
        }

#if 0

  printf("[0]=%p, [1] =  %p\n" , __builtin_frame_address(0), __builtin_frame_address(1));
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
int main()
{
  printf("main=%p g=%p f=%p\n", main, g, f);
  g();
}

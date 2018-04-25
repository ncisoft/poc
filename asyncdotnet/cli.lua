#!/usr/bin/env lua

local socket = require("socket")

function call_exit(host, port, cmd)
  assert(host, "host must be not null "..host)
  assert(port, "port must be not null "..port)
  cmd = cmd or "help";
  if cmd == "help" then
    print("usage: ./cli.lua gcstatus | gccollect | ping | exit ")
    os.exit(0)
  end
  local s, out,errmsg
  s = assert(socket.tcp());
  s:settimeout(0.1, 'b')
  out = s:connect(host, port) or assert(false, "connect failure to " ..host ..":" ..port);

  out,errmsg = s:send(cmd .."\n")
  print("has sent cmd=`" ..cmd .."` to " ..host .. ":" ..port)
  if not out then
    print(errmsg)
    os.exit(1)
  end
  while (out) do
    out, errmsg = s:receive("*l")
    if out then print(out) end
  end
  s:close();

end

call_exit("127.0.0.1", 3333, arg[1]);

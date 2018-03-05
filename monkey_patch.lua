#!/usr/bin/env lua

require "commons.print"
local dns=require "dns"
--local coxpcall = require "coxpcall"
local socket=require "socket"
local ev=require "ev"
local loop = ev.Loop.default
local pl = require 'pl.pretty' 
local coutils = require "coutils"
local base= _G

local mk_tcp_tbl = 
{
  connect = true,
  close = true,
  shutdown = true,
  accept = true,
  receive = true,
  send = true,
}

local M = mk_tcp_tbl
local rM = {}
local ctx = { main_co = base.coroutine.running() }


function M.mk_connect(s, address, port)
  xprint("monkey_patch connect()")
  local co_self = coroutine.running()
  xprint("co=%s, main_co=%s", co_self, ctx.main_co)
  assert(co_self ~= ctx.main_co, "!!! could not be runned under main coroutine context")
  local  on_connect = function()
    xprint("connected to %s:%s", address, port)
    io_watcher:stop(loop)

    coroutine.resume(co_self)
  end

  io_watcher = ev.IO.new(on_connect, s:getfd(), ev.WRITE)
  io_watcher:start(loop)
  s:_connect(address, port)
  coroutine.yield()
  -- waiting for resumed
  M.__intercept_socket(s)
end

function M.mk_close(s)
  xprint("monkey_patch close()")
  s:_close()
end

function M.mk_receive(s, pattern, prefix)
  xprint("monkey_patch receive()")
  local co_self = coroutine.running()
  xprint("co=%s, main_co=%s", co_self, ctx.main_co)
  assert(co_self ~= ctx.main_co, "!!! could not be runned under main coroutine context")
  local  on_receive = function(loop, io_watcher, revents)

    local data, errmsg, partial = s:_receive(pattern, prefix)

    if (not data and errmsg == "closed") then
      io_watcher:stop(loop)
      xprint("closed")
    end

    coroutine.resume(co_self, data, errmsg, partial)
  end

  if not ctx[s] then
    ctx[s]={}
  end
  if not ctx[s].rwatcher then
    local io_watcher = ev.IO.new(on_receive, s:getfd(), ev.READ)
    io_watcher:start(loop)
    ctx[s].rwatcher = io_watcher
  end

  local out = {coroutine.yield()}
  return unpack(out)
end

function M.mk_send(s, data, ...)
  s:_send(data, ...)
  xprint("monkey_patch send()")
  xprint("--\n%s",data)
end

-- init rM
if true then
  local fname, fpointer
  for  fname, fpointer in pairs(M) do
    if type(fpointer) == "function" then
      fname = string.gsub(fname, "(%w+)_(%w+)", "%2")
      rM[ fpointer ]  = fname
    end
  end
end

local function __intercept_meth(mt, patch_msgs, patched_f)
  local mt_index, f, _f, f_name
  assert(patched_f, "patched_f is nil")
  assert(type(patched_f) == "function", "type(patched_f) is not function ")
  mt_index = mt.__index
  f_name = rM[ patched_f ]
  assert(f_name, "patched_fname is nil")
  f = f_name
  _f = "_" .. f

  mt_index[_f]= mt_index[f_name]
  mt_index[f_name]= patched_f
  table.insert(patch_msgs, string.format("monkey_patch %s():%s", f_name, patched_f))
end

local function __intercept_metatable()
  local s
  s = socket.tcp()
  M.__intercept_socket(s)
  s:_close()
end

--[[
--
-- class=tcp{master}
-- class=tcp{client}
--
--]]
function M.__intercept_socket(s)
  xprint("socket.metatable")
  assert(s, "socket must not be nil")
  --pl.dump(rrs)
  local patch_msgs = {""}
  local tab4 = string.rep(" ", 2)
  local mt, mt_index

  mt = getmetatable(s)
  xprint("mt=%s\n", s)
  if not mt.__index.intercepted then
    __intercept_meth(mt, patch_msgs, M.mk_connect)
    __intercept_meth(mt, patch_msgs, M.mk_close)
    __intercept_meth(mt, patch_msgs, M.mk_send)
    __intercept_meth(mt, patch_msgs, M.mk_receive)
    mt.__index.intercepted = true
  end

  xprint("monkey_patch for %s as below \n%s\n", mt.__index.class, table.concat(patch_msgs, "\n"..tab4))
  --pl.dump(mt)
  xprint("send=%s\n", M.mk_send)

end


local function __TRACKBACK__(errmsg)
	local track_text = base.debug.traceback(base.tostring(errmsg),1);

	base.print("---------------------------------------- TRACKBACK ----------------------------------------");
	base.print(track_text, "\n\tLUA ERROR");
	base.print("---------------------------------------- TRACKBACK ----------------------------------------");
	--local exception_text = "LUA EXCEPTION\n" .. track_text;
	base.os.exit(0)
	return false;
end

local function co_f(f, ...)
  local args = { f, ...  }
  xpcall(
  function ()
    f(base.unpack(args, 2))
  end,
  __TRACKBACK__
  )
end
local function spawn3(f, ...)
  local co = base.coroutine.create(co_f)
  base.coroutine.resume(co, f, ...)
end

local function utest_cof()
  println()
  s=socket.tcp()
  s:settimeout(0)
  s:connect("192.168.1.9", 80)
  xprint("connected...")
  local req={
    "GET / HTTP/1.0", 
    "Host: 192.168.1.9", 
    "Accept: */*",
    "",
    ""
  }
  req = table.concat(req, "\r\n")
  --xprint("\n%s",req)
  s:send(req)
  local msg=s:receive(2048)
  xprint("\nmsg=%s",msg)
  mt = getmetatable(s)
  --pl.dump(mt)
  xprint("send=%s", M.mk_send)
  xprint("_send=%s", mt.__index.send)
  xprint("class=%s", mt.__index.class)

end

local function utest()
  spawn3(utest_cof)
  loop:loop()
end
__intercept_metatable()
utest()

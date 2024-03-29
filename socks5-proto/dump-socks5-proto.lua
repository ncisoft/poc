#!/usr/bin/env lua

local _xprint = require ("commons.print")
local dump = require 'pl.pretty'.dump
require("commons.getopt")
local socket = require("socket")
local char = string.char

-- magic numbers
local SOCKS5 = 0x05
local NUMBER_OF_AUTH_METHODS = 0x01
local NO_AUTHENTICATION = 0x00
local TCP_CONNECTION = 0x01
local RESERVED = 0x00
local IPv4 = 0x01
local DOMAIN_NAME = 0x03
local IPv6 = 0x04

local REQUEST_GRANTED = 0x00
local CONN_ERRORS = {
  [0x01] = 'general failure',
  [0x02] = 'connection not allowed by ruleset',
  [0x03] = 'network unreachable',
  [0x04] = 'host unreachable',
  [0x05] = 'connection refused by destination host',
  [0x06] = 'TTL expired',
  [0x07] = 'command not supported / protocol error',
  [0x08] = 'address type not supported',
}

local s = socket.tcp()
local function init_socks5_server()
  s = socket.tcp()
  socks5_host = "192.168.1.8"
  socks5_port = 10832

  assert(s:connect(socks5_host, socks5_port), "connect fail")
  xprint("... socks5 server=%s:%d\n", socks5_host, socks5_port)
  -- step(1): init authentification
  local auth_req = char(SOCKS5, NUMBER_OF_AUTH_METHODS, NO_AUTHENTICATION)
  s:send(auth_req)
  dump_hex_inline("[c00]init_auth", auth_req)
  local auth_response = s:receive(2)
  assert(auth_response, "auth_response is nil")
  if auth_response ~= char(SOCKS5, NO_AUTHENTICATION) then
    return nil, "Socks5 authentification failed"
  end
  dump_hex_inline("[s00]ack_auth ", auth_response)
end

local function sprint(fmt, ...)
  local line=_G.sprint(fmt, ...)
  print(line)
end


local function connect_hostname(host)
  local host_length = #host
  local port = 80
  local port_big_endian = char( math.floor(port / 256), port % 256 )
  local conn_req = char(SOCKS5, TCP_CONNECTION, RESERVED, DOMAIN_NAME, host_length) .. host .. port_big_endian
  dump_hex_inline("[c00]conn_req ", conn_req)
  s:send(conn_req)
  local conn_response = s:receive(3)
  assert(conn_response, "conn_response is nil")
  if conn_response ~= char(SOCKS5, REQUEST_GRANTED, RESERVED) then
    dump_hex_inline("  [c]conn_resp", conn_response)
    local status = conn_response:byte(2)
    local message = CONN_ERRORS[status] or 'Unknown error'
    assert(nil, message)
  end
  dump_hex_inline("[s00]conn_ack ", conn_response)
  local ver = conn_response:byte(1)
  local rep = conn_response:byte(3)
  sprint("[s00]conn_ack : %s ver = 0x%02x connect_rep = %s", " ", ver, rep == 0x0)
  -- pop address
  local addr_type = s:receive(1)
  dump_hex_inline("[s00]addr_type", addr_type)
  if addr_type == char(DOMAIN_NAME) then
    local addr_length = addr_type:byte(1)
    dump_hex_inline("[s03]addr_len ", "" ..addr_length)
    local remote_addr = s:receive(addr_length)
    dump_hex_inline("[s03]ack_addrx", remote_addr)
  elseif addr_type == char(IPv4) then
    local remote_addr = s:receive(4)
    dump_hex_inline("[s01]ack_addr ", remote_addr)
  elseif addr_type == char(IPv6) then
    s:receive(16)
  else
    assert( nil, 'Bad address type: ' .. string.byte(addr_type) )
  end
  -- pop port
  local remote_port = s:receive(2)
  dump_hex_inline("[s00]ack_port ", remote_port)

end

local function connect_ipv4(ip)
  local port = 80
  local port_big_endian = char( math.floor(port / 256), port % 256 )
  local conn_req = char(SOCKS5, TCP_CONNECTION, RESERVED, IPv4) .. ip .. port_big_endian
  dump_hex_inline("[c00]conn_req ", conn_req)
  s:send(conn_req)
  local conn_response = s:receive(3)
  assert(conn_response, "conn_response is nil")
  if conn_response ~= char(SOCKS5, REQUEST_GRANTED, RESERVED) then
    dump_hex_inline("  [c]conn_resp", conn_response)
    local status = conn_response:byte(2)
    local message = CONN_ERRORS[status] or 'Unknown error'
    assert(nil, message)
  end
  dump_hex_inline("[s00]conn_ack ", conn_response)
  -- pop address
  local addr_type = s:receive(1)
  dump_hex_inline("[s00]addr_type", addr_type)
  if addr_type == char(DOMAIN_NAME) then
    local addr_length = addr_type:byte(1)
    dump_hex_inline("[s03]addr_len ", "" ..addr_length)
    local remote_addr = s:receive(addr_length)
    dump_hex_inline("[s03]ack_addrx", remote_addr)
  elseif addr_type == char(IPv4) then
    local remote_addr = s:receive(4)
    dump_hex_inline("[s01]ack_addr ", remote_addr)
  elseif addr_type == char(IPv6) then
    s:receive(16)
  else
    assert( nil, 'Bad address type: ' .. string.byte(addr_type) )
  end
  -- pop port
  local remote_port = s:receive(2)
  dump_hex_inline("[s00]conn_port", remote_port)

end

_G.xopts =
{
	decl =
	{
		{ 'h' , "host"    , true , "www.baidu.com", function(k,v)  return v ;end } ,
		{ 'p' , "port"    , true  , 80, function(k,v)  return k,v; end } ,
		{ 't' , "type"    , true  , "any", function(k,v)  return k,v; end } ,
		{ 'd' , "debug"   , false , true, function(k,v)  return k,v; end }
	},
	--options = {},
	--args = {}, -- remaining args
	help_text = ""
}
local function validate_opts()
  return true
end

xparse_opt()
_G.xopts.decl = nil
dump(_G.xopts)
assert( validate_opts(), "xopts is invalid" );

print("")
SECTION("... check connect socks5 server ..\n")
init_socks5_server()
print("")
SECTION("... check connect hostname via socks5 server ..\n")
connect_hostname("[2001:da8:20d:7042::f2]")

print("")
SECTION("... done ...\n")
os.exit(0)

print("")
s:close()
SECTION("... check connect socks5 server ..\n")
init_socks5_server()
print("")
SECTION("... check connect IPv4 via socks5 server ..\n")
connect_ipv4(char(0x3D, 0x87, 0xA9, 0x7D))
print("")
SECTION("... done ...\n")

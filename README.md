# Evelyn Digital Library

The ASP.NET Core implementation of Evelyn Digital Library.

## Configuration

The default `max_allowed_packet` size is 4MB, which is not enough. Edit the server
configuration file (on Windows 10 it's at `\ProgramData\MySQL\MySQL Server 8.0\my.ini`)
to change it to 500MB.
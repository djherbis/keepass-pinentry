keepass-pinentry
==========

[![Release](https://img.shields.io/github/release/djherbis/keepass-pinentry.svg)](https://github.com/djherbis/keepass-pinentry/releases/latest)
[![experimental](https://badges.github.io/stability-badges/dist/experimental.svg)](http://github.com/badges/stability-badges)
[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](LICENSE.txt)
[![Build Status](https://github.com/djherbis/keepass-pinentry/actions/workflows/build-keepass-pinetry.yml/badge.svg)](https://github.com/djherbis/keepass-pinentry/actions/workflows/build-keepass-pinetry.yml)

Tired of typing in your GPG password? Have Keepass do it for you!
With this Keepass plugin installed and gpg-agent configured you can have Keepass respond to pinentry requests for you.

Installation
----------

Clone this repo, do this somewhere it can reside permanently since we add ENV vars which point to this dir.

Grab a copy of KeepassPinentry.dll and stdproxy.exe from [Releases](https://github.com/djherbis/keepass-pinentry/releases) (or build them yourself).

Run the install.cmd (auto adds the plugin ddl to the keepass dir).

In Keepass:
* Add an Entry named "GPG" whose password is your GPG password (the one you want pinentry to use).
* Add an Entry named "TLSKEY" whose password is the certificate password you created, and add the certificate.p12 file as a binary to this entry.

How it works
----------

The idea is really simple, we tell gpg-agent to talk to stdproxy as its pinentry program.
It uses a simple text protocol to communicate with the pinentry program over STDIN/STDOUT.
Stdproxy makes a TCP connection (wrapped in SSL) to the keepass-pinentry plugin which is
running a local server. This TCP connection is used to forward the STDIN written by gpg-agent to stdproxy to keepass-pinentry which then responds over TCP back to stdproxy's STDOUT which is read by gpg-agent. Keepass-pinentry implements the bare minimum parts of the pinentry protocol in order to respond to the pinentry request.

Building locally / Development
----------

**KeepassPinentry.dll:**

Dependencies:
* Docker for Windows or dotnet

You can rebuild the DLL by running build.cmd, or running dotnet inside KeepassPinentry/.

**stdproxy.exe:**

Dependencies:
* Go

cd into stdproxy/ and run "go build"

Notes
----------

The SSL encryption here is probably silly since this is only ever intended to be run with a local keepass-pinentry server & gpg-agent, but it was fun adding the encryption layer.

The code is really rough because this was just a personal tool, so forgive me for that!

Future work
----------

* Fallback to non-SSL TCP when certificate not present.
* Add tests

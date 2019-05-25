keepass-pinentry
==========

Tired of typing in your GPG password? Have Keepass do it for you!
With this Keepass plugin installed and gpg-agent configured you can have Keepass respond to pinentry requests for you.

Installation
----------

Clone this repo, do this somewhere it can reside permanently since we add ENV vars which point to this dir.

Run the install.cmd (auto adds the plugin ddl to the keepass dir).

Add a Keepass Entry called "GPG" whose password is your GPG password.
Add a Keepass Entry called "TLSKEY" whose password is the certificate password you created, and add the certificate.pem file as a binary to this entry.

How it works
----------

The idea is really simple, we tell gpg-agent to talk to stdproxy as its pinentry program.
It uses a simple text protocol to communicate with the pinentry program over STDIN/STDOUT.
Stdproxy makes a TCP connection (wrapped in SSL) to the keepass-pinentry plugin which is
running a local server. This TCP connection is used to forward the STDIN written by gpg-agent to stdproxy to keepass-pinentry which then responds over TCP back to stdproxy's STDOUT which is read by gpg-agent. Keepass-pinentry implements the bare minimum parts of the pinentry protocol in order to respond to the 

Building locally / Development
----------

Dependencies:
* Docker for Windows

You can rebuild the DLL by running build.cmd.

Notes
----------

The SSL encryption here is probably silly since this is only ever intended to be run with a local keepass-pinentry server & gpg-agent, but it was fun adding the encryption layer.

The code is really rough because this was just a personal tool, so forgive me for that!

Future work
----------

* Fallback to non-SSL TCP when certificate not present.
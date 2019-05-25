package main

import (
	"crypto/tls"
	"crypto/x509"
	"fmt"
	"io"
	"io/ioutil"
	"log"
	"net"
	"os"
)

func main() {
	conn := dial()
	defer conn.Close()

	go io.Copy(os.Stdout, conn)
	io.Copy(conn, os.Stdin)
}

func dial() net.Conn {
	port := fallback(os.Getenv("STDPROXY_PORT"), "500")
	addr := fmt.Sprintf("localhost:%s", port)

	config := &tls.Config{
		InsecureSkipVerify: true,
		RootCAs:            getCertPool(),
	}

	conn, err := tls.Dial("tcp", addr, config)
	if err != nil {
		log.Fatal(err)
	}

	return conn
}

func getCertPool() *x509.CertPool {
	// TODO(djherbis): embed this
	file := os.Getenv("STDPROXY_CERT")
	data, err := ioutil.ReadFile(file)
	if err != nil {
		log.Fatal(err)
	}

	roots := x509.NewCertPool()
	ok := roots.AppendCertsFromPEM(data)
	if !ok {
		log.Fatal("failed to parse root certificate")
	}

	return roots
}

func fallback(first, second string) string {
	if first != "" {
		return first
	}
	return second
}

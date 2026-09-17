# NKafka

[Русский](../ru/nkafka.md)

NKafka is a .NET client library for Apache Kafka. It is designed to support producing and consuming messages and integration with .NET applications. The sections below describe the intended architecture; feature readiness must be checked against the relevant specifications and tests.

## Cluster

The cluster is the central component and entry point for working with Kafka. An application can manage several clusters independently. After connecting, it can create producers and consumers from the cluster.

The cluster also provides administrative operations, such as creating and deleting topics and managing access control lists.

Closing the cluster releases its internal resources and transport connections. Clients that depend on it cannot continue normal operation afterwards.

Cluster configuration contains connection, security, protocol version, broker discovery, and topology refresh settings. Clients created from the cluster use this shared connection context throughout their lifetime.

## Connection pool

NKafka's design shares broker connections at the cluster level to avoid unnecessary connections for each producer. A Kafka cluster can require connections to several brokers; sharing does not mean that every broker is accessed through one socket. Dedicated consumer connections are used where required by the client's interaction model.

Shared resources are intended to reduce connection overhead. The network layer aims to use socket events and a limited number of worker threads or the .NET thread pool rather than creating unnecessary threads. These are architectural goals, not a measured performance comparison with other clients.

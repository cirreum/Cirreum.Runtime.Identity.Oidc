# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Updated

- Updated NuGet packages (Cirreum spine 4.2.0 wave repins).

## [1.0.9] - 2026-07-29

### Updated

- Updated NuGet packages.

## [1.0.8] - 2026-07-24

### Fixed

- The README's provisioner walkthrough taught the pre-`Cirreum.IdentityProvider 2.0.0` mental
  model: it set a `Roles` property on the app's user type without showing the
  `IProvisionedIdentity.Claims` projection that actually mints it. Since roles stopped being a
  privileged framework concept, setting `Roles` alone puts nothing in the token — a silent
  no-op for anyone copying the sample. The README now leads with the provisioned-identity type
  and its `Claims` projection. Documentation only; no API or behavior change.

## [1.0.7] - 2026-07-23

### Updated

- Updated NuGet packages.

## [1.0.6] - 2026-07-20

### Updated

- Updated NuGet packages.

## [1.0.5] - 2026-07-19

### Updated

- Updated NuGet packages.

## [1.0.4] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.3] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.2] - 2026-05-01

### Updated
- Updated NuGet packages.


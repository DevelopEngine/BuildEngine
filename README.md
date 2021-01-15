# BuildEngine

> Despite the name, not really for builds.

This library is _extremely_ work-in-progress and not likely to be suitable for many use cases yet.

## Introduction

BuildEngine is a general purpose library for creating and using on-demand disposable ""build"" environments. While the project name (and most of the code) refers to this in the context of building things, the library is actually designed to be used for anything that requires predictable semi-clean environments used for doing *something*. 

Whether that's running a script, editing a file in isolation or actually building something, you _should_ be able to use the mechanics and primitives in this lib to make that easier.  
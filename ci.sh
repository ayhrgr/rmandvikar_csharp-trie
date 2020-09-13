#!/bin/sh

dotnet restore
dotnet clean -c Release
# netcoreapp3.0 only as test csproj also targets others
dotnet build Trie.sln \
	-c Release \
	-f netcoreapp3.1
dotnet test tests/rm.TrieTest/rm.TrieTest.csproj \
	-c Release --no-build --no-restore \
	-f netcoreapp3.1 \
	-v normal \
	--filter "TestCategory!=very.slow"

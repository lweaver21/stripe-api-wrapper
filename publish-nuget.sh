#!/bin/bash

# StripeApiWrapper NuGet Package Publisher
# Usage: ./publish-nuget.sh [API_KEY]
# Or set NUGET_API_KEY environment variable

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTPUT_DIR="$SCRIPT_DIR/nupkg"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"

# Get API key from argument or environment variable
API_KEY="${1:-$NUGET_API_KEY}"

if [ -z "$API_KEY" ]; then
    echo "Error: NuGet API key is required."
    echo "Usage: ./publish-nuget.sh <API_KEY>"
    echo "   Or: export NUGET_API_KEY=<your-key> && ./publish-nuget.sh"
    exit 1
fi

echo "=== Building solution in Release mode ==="
dotnet build "$SCRIPT_DIR" -c Release

echo ""
echo "=== Running tests ==="
dotnet test "$SCRIPT_DIR" -c Release --no-build

echo ""
echo "=== Creating NuGet package ==="
rm -rf "$OUTPUT_DIR"
dotnet pack "$SCRIPT_DIR/src/StripeApiWrapper" -c Release --no-build -o "$OUTPUT_DIR"

echo ""
echo "=== Publishing to NuGet.org ==="
for package in "$OUTPUT_DIR"/*.nupkg; do
    if [ -f "$package" ]; then
        echo "Publishing: $(basename "$package")"
        dotnet nuget push "$package" --api-key "$API_KEY" --source "$NUGET_SOURCE" --skip-duplicate
    fi
done

echo ""
echo "=== Done! ==="
echo "Package published successfully to NuGet.org"

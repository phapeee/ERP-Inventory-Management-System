#!/bin/bash

REPLACE=false
MODULE_NAME=""

# Parse command-line arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --replace) REPLACE=true; shift ;;
        *) MODULE_NAME="$1"; shift ;;
    esac
done

if [ -z "$MODULE_NAME" ]; then
  echo "Usage: $0 [--replace] <module_name>"
  exit 1
fi

MODULE_DIR="Modules/$MODULE_NAME"
SOLUTION_FILE="ERP-Inventory-Management-System.sln"

if [ -d "$MODULE_DIR" ]; then
  if [ "$REPLACE" = true ]; then
    read -p "Warning: A module with the name '$MODULE_NAME' already exists. This will permanently remove the existing module. Do you want to proceed? [y/N] " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
      echo "Removing existing module from solution..."
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Domain/$MODULE_NAME.Domain.csproj"
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Domain.Tests/$MODULE_NAME.Domain.Tests.csproj"
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Application/$MODULE_NAME.Application.csproj"
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Application.Tests/$MODULE_NAME.Application.Tests.csproj"
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Infrastructure/$MODULE_NAME.Infrastructure.csproj"
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Infrastructure.Tests/$MODULE_NAME.Infrastructure.Tests.csproj"
      dotnet sln $SOLUTION_FILE remove "$MODULE_DIR/$MODULE_NAME.Presentation/$MODULE_NAME.Presentation.csproj"
      
      echo "Removing existing module directory..."
      rm -rf "$MODULE_DIR"
    else
      echo "Aborting."
      exit 0
    fi
  else
    echo "Error: A module with the name '$MODULE_NAME' already exists. Use the --replace flag to overwrite."
    exit 1
  fi
fi

echo "Creating new module..."
dotnet new erp-module -n $MODULE_NAME -o $MODULE_DIR

echo "Adding new module to solution..."
# Add the new projects to the solution
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Domain/$MODULE_NAME.Domain.csproj"
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Domain.Tests/$MODULE_NAME.Domain.Tests.csproj"
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Application/$MODULE_NAME.Application.csproj"
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Application.Tests/$MODULE_NAME.Application.Tests.csproj"
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Infrastructure/$MODULE_NAME.Infrastructure.csproj"
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Infrastructure.Tests/$MODULE_NAME.Infrastructure.Tests.csproj"
dotnet sln $SOLUTION_FILE add "$MODULE_DIR/$MODULE_NAME.Presentation/$MODULE_NAME.Presentation.csproj"

echo "Module '$MODULE_NAME' created successfully."

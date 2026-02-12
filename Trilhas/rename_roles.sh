#!/bin/bash

# Script to rename roles in the Trilhas application
# Coordenador -> GEDTH
# Secretaria -> GESE

echo "🔄 Starting role rename process..."
echo ""

# Find all .cs and .cshtml files
echo "📁 Searching for files to update..."
FILES=$(find . -type f \( -name "*.cs" -o -name "*.cshtml" \) ! -path "*/bin/*" ! -path "*/obj/*" ! -path "*/.git/*")

TOTAL_FILES=$(echo "$FILES" | wc -l)
echo "Found $TOTAL_FILES files to process"
echo ""

CHANGED_FILES=0

# Process each file
for file in $FILES; do
    # Check if file contains either role name
    if grep -q "Coordenador\|Secretaria" "$file" 2>/dev/null; then
        echo "✏️  Updating: $file"
        
        # Create backup
        cp "$file" "$file.bak"
        
        # Replace role names
        # Use sed with backup and then remove backup if successful
        sed -i.tmp \
            -e 's/Coordenador/GEDTH/g' \
            -e 's/Secretaria/GESE/g' \
            "$file"
        
        # Remove temporary file created by sed
        rm -f "$file.tmp"
        
        CHANGED_FILES=$((CHANGED_FILES + 1))
    fi
done

echo ""
echo "✅ Role rename complete!"
echo "📊 Summary:"
echo "   - Total files scanned: $TOTAL_FILES"
echo "   - Files modified: $CHANGED_FILES"
echo ""
echo "⚠️  Backup files created with .bak extension"
echo "💡 To remove backups after verification: find . -name '*.bak' -delete"
echo ""
echo "🔍 Next steps:"
echo "   1. Review the changes with: git diff"
echo "   2. Test the application thoroughly"
echo "   3. Update the database roles if needed"
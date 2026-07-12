#!/bin/bash

# Prompt user for new project name
read -p "Enter the new project name: " projectName

# Convert project name to various cases for replacements
# Convert to lowercase
lowerName=$(echo "$projectName" | awk '{print tolower($0)}')

# Convert to uppercase
upperName=$(echo "$projectName" | awk '{print toupper($0)}')

# Convert to PascalCase (capitalize each word, remove spaces/underscores)
pascalName=$(echo "$projectName" | sed -r 's/[_ ]+([a-z])/\U\1/g' | sed -r 's/^[a-z]/\U&/' | sed 's/ //g')

# Convert to kebab-case (replace spaces/underscores with hyphens, lowercase)
kebabName=$(echo "$projectName" | sed -r 's/[_ ]+/-/g' | awk '{print tolower($0)}')

# Convert to camelCase (capitalize words after the first, remove spaces/underscores)
camelName=$(echo "$projectName" | sed -r 's/[_ ]+([a-z])/\U\1/g' | sed -r 's/^[A-Z]/\L&/' | sed 's/ //g')

echo "lowerName: '$lowerName'"
echo "upperName: '$upperName'"
echo "pascalName: '$pascalName'"
echo "kebabName: '$kebabName'"
echo "camelName: '$camelName'"

# Function to replace text preserving the case
replace_text() {
  local input=$1

  # Skip binary files so sed -i cannot corrupt them (e.g. images, fonts).
  grep -Iq . "$input" 2>/dev/null || return 0

  # Replace MicroserviceTemplate variations (spaced form covers display text like
  # "BytLabs Microservice Template" in prose/README).
  sed -i \
      -e "s/MicroserviceTemplate/${pascalName}/g" \
      -e "s/microserviceTemplate/${camelName}/g" \
      -e "s/microservice-template/${kebabName}/g" \
      -e "s/microservicetemplate/${lowerName}/g" \
      -e "s/MICROSERVICETEMPLATE/${upperName}/g" \
      -e "s/Microservice Template/${projectName}/g" \
      -e "s/microservice template/${lowerName}/g" "$input"
}

# Function to recursively rename directories and files. Build output (bin/obj),
# dependencies (node_modules), build artifacts (dist), and VCS/IDE folders are skipped
# at ANY depth so the script only touches source — not the ~33k files those dirs hold.
rename_files_and_dirs() {
  # Process directories first (deepest-first via -depth, so children are renamed before
  # their parents). -depth is incompatible with -prune, so exclude by path at any depth.
  find . -depth -type d \
    ! -path './.git' ! -path './.git/*' \
    ! -path './.vs' ! -path './.vs/*' \
    ! -path '*/bin' ! -path '*/bin/*' \
    ! -path '*/obj' ! -path '*/obj/*' \
    ! -path '*/node_modules' ! -path '*/node_modules/*' \
    ! -path '*/dist' ! -path '*/dist/*' | while read -r dir; do
    newDir=$(echo "$dir" | sed -r \
      -e "s/MicroserviceTemplate/${pascalName}/g" \
      -e "s/microserviceTemplate/${camelName}/g" \
      -e "s/microservice-template/${kebabName}/g" \
      -e "s/microservicetemplate/${lowerName}/g" \
      -e "s/MICROSERVICETEMPLATE/${upperName}/g")

    if [ "$dir" != "$newDir" ]; then
      mv "$dir" "$newDir"
    fi
  done

  # Process files after directories. Prune the excluded dirs entirely (fast; never descends
  # into node_modules) and skip the script itself.
  find . \
    -type d \( -name .git -o -name .vs -o -name bin -o -name obj -o -name node_modules -o -name dist \) -prune \
    -o -type f ! -name set-project-name.sh -print | while read -r file; do
    newFile=$(echo "$file" | sed -r \
      -e "s/MicroserviceTemplate/${pascalName}/g" \
      -e "s/microserviceTemplate/${camelName}/g" \
      -e "s/microservice-template/${kebabName}/g" \
      -e "s/microservicetemplate/${lowerName}/g" \
      -e "s/MICROSERVICETEMPLATE/${upperName}/g")

    if [ "$file" != "$newFile" ]; then
      mv "$file" "$newFile"
    fi
  done
}

# Replace content in files, pruning build/dependency/VCS dirs and skipping the script itself.
process_files() {
  find . \
    -type d \( -name .git -o -name .vs -o -name bin -o -name obj -o -name node_modules -o -name dist \) -prune \
    -o -type f ! -name set-project-name.sh -print | while read -r file; do
    replace_text "$file"
  done
}

# Run the renaming and content replacement functions
rename_files_and_dirs
process_files

echo "All occurrences of 'MicroserviceTemplate' have been replaced with '${projectName}' while preserving the case."

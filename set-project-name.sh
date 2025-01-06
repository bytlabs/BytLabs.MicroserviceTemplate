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

  # Replace MicroserviceTemplate variations
  sed -i \
      -e "s/MicroserviceTemplate/${pascalName}/g" \
      -e "s/microserviceTemplate/${camelName}/g" \
      -e "s/microservice-template/${kebabName}/g" \
      -e "s/microservicetemplate/${lowerName}/g" \
      -e "s/MICROSERVICETEMPLATE/${upperName}/g" "$input"
}

# Function to recursively rename directories and files, excluding certain directories and the script itself
rename_files_and_dirs() {
  # Process directories first (in reverse order of depth)
  find . -depth -type d \
    ! -path './.git' \
    ! -path './.vs*' \
    ! -path './bin*' \
    ! -path './obj*' \
    ! -path './set-project-name.sh' | while read -r dir; do
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

  # Process files after directories
  find . -type f \
    ! -path './.git/*' \
    ! -path './.vs*' \
    ! -path './bin*' \
    ! -path './obj*' \
    ! -path './set-project-name.sh' | while read -r file; do
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

# Replace content in files, excluding certain directories and the script itself
process_files() {
  find . -type f \
    ! -path './.git/*' \
    ! -path './.vs*' \
    ! -path './bin*' \
    ! -path './obj*' \
    ! -path './set-project-name.sh' | while read -r file; do
    replace_text "$file"
  done
}

# Run the renaming and content replacement functions
rename_files_and_dirs
process_files

echo "All occurrences of 'MicroserviceTemplate' have been replaced with '${projectName}' while preserving the case."

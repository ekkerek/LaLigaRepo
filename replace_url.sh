#!/bin/bash
echo "Running replace_url.sh script..."
sed -i 's|"API_URL" : "URL"|"API_URL" : "http://localhost:5000"|g' appsettings.Production.json
echo "Script execution completed."

nginx -g 'daemon off;'
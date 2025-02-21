echo "Deploying to Master Source Code except Site..."
# Ensure you're in the project root
git checkout master || exit 1

# Commit and push changes
git add .
git commit -m "Update Master"
git push -u origin master

# Switch back to master (optional)
# git checkout master
echo "Deployment completed!"

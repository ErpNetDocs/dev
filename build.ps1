param(
    [string]$docfx
)

Write-Host Setting up git remote ssh...

git config remote.ssh.url
if ($lastexitcode -eq 1) {
    git remote add ssh git@github.com:ErpNetDocs/dev.git
}

git checkout master
Write-Host Undoing local changes...
git reset --hard HEAD^

Write-Host Building docfx
"$docfx build"

Write-Host Upload Changes to Github
git add -A
git push ssh master

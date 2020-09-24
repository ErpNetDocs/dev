param(
    [string]$docfx
)

$git_bin = "C:\Program Files\Git\usr\bin"

Write-Host "user is $env:USERNAME"

if (test-path "$home\.ssh\id_rsa.pub") {
    Write-Host "Has public key:"
    Get-Content "$home\.ssh\id_rsa.pub"
} else {
    Write-Host "Doesn't have public key"
}

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

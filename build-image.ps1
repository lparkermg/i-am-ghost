param(
    [string]$version = "0.0.0"
)

& docker image build -t lp-soft/i-am-ghost:$version --force-rm -f docker/Dockerfile .


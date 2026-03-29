podman build -t dotnet-dev-env -f Containerfile

podman run -it --rm \
  -v $(pwd):/workspace \
  -v $HOME/.config/nvim:/root/.config/nvim \
  -v $HOME/.tmux:/root/.tmux \
  -v $HOME/.tmux.conf:/root/.tmux.conf \
  dotnet-dev-env

{ pkgs }: {
  deps = [
    pkgs.dotnet-sdk_8
  ];

  env = {
    DOTNET_CLI_HOME = "/home/runner/.dotnet";
    DOTNET_ROOT = "${pkgs.dotnet-sdk_8}";
    LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath [
      pkgs.openssl
      pkgs.icu
    ];
  };
}

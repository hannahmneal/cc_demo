<!--
Release PR: master <- dev
Title: "Release vX.Y.Z"
Merged work and issues are listed in the GitHub release notes, not here.
-->

## Release vX.Y.Z

**Version bump:** <!-- MAJOR (breaking API change) / MINOR (new feature) / PATCH (fix) -->

## Before merging
- [ ] All required checks pass (`test`, security scans, CodeQL)
- [ ] Database migrations in this release, if any, are safe to apply to prod
- [ ] Anything to do by hand before or after the deploy is noted below

<!-- Manual steps (e.g. new secrets, env vars, Render/Neon/Cloudflare settings). Delete if none. -->

## After merging
- [ ] Publish the release from `master` (this deploys to prod):
      `gh release create vX.Y.Z --target master --generate-notes`
- [ ] Confirm the deploy workflow succeeded and https://cc-demo.redqueen.run is healthy
- [ ] Record any gotchas from the merge or release as comments on this PR

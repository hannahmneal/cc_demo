# CC_Demo

A fun little project I'm working on.

Live at: https://cc-demo.redqueen.run


## TLDR; Deployment Architecture (Backend)

A Microservice API hosted on Render with Cloudflare sitting in front of it as a DNS + proxy + security layer. Once a browser/client request reaches the container, the API code connects over the network to read/write data to the serverless database on Neon. 

Terraform scaffolds the infrastructure. In order to avoid tf state collisions on successive CI runs, Cloudflare's R2 Object Storage bucket is used for shared state storage.

GitHub Actions provides easy credential delivery and deployment: publishing a GitHub release sets the deployment into motion. Merging to `master` alone does not deploy.

### Releasing

1. Open and merge the release PR (`master` <- `dev`) using the release template: `gh pr create --base master --head dev --template release.md --label release`. Issues and merged work are listed in the generated release notes, not the PR. Notes are grouped by PR label (see `.github/release.yml`), so label feature PRs `enhancement`, `bug`, `documentation`, etc.
2. Create a release from `master`, e.g. `gh release create v1.2.0 --target master --generate-notes` (or **Releases > Draft a new release** in GitHub). Use `vMAJOR.MINOR.PATCH` tags.
3. Publishing the release runs tests, builds the image (tagged with the version, the commit SHA and `latest`), applies Terraform and triggers the Render deploy.

Notes:
- The workflow refuses to deploy a release whose commit is not on `master`.
- Pre-releases are not deployed.
- `v*` tags are protected: only repo admins can move or delete them.
- **Rollback:** open the Actions run for an earlier release and choose **Re-run all jobs**. It rebuilds that version, points `latest` back at it and redeploys.

### 🚩 Gotchas 🚩

- The services for this project were chosen specifically for their free tiers. This is why there aren't separate deployment environments (dev/qa/prod) for this project. If you want me to implement more environments, hire me or give me money.
- The `render_web_services.cc_demo_api` is pinned to the `:latest` tag because a known bug in the Render Terraform provider breaks *updates* to the free-tier service being used. Therefore, a Render Deploy Hook (webhook) tells Render to "go pull latest again", sidestepping the bug entirely. For more information, please see https://github.com/render-oss/terraform-provider-render/issues/80 or the comment in `terraform/render.tf`.
- Neon's free tier caps PITR history at 6h, forcing `history_retention_seconds = 21600` instead of the provider default (24h).
- Npgsql cannot parse Neon's `postgres://` URI format; therefore, the connection string is rebuilt from individual Neon attributes (i.e., `Host=...;Port=...;...`) in `neon.tf`.



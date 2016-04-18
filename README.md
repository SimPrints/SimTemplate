# SimTemplate
##Sam's super awesome template tool.

### TODOs
- [x] Scanner select drop down
- [ ] Help icon
  * Show a basic image with a brief explanation of How To
- [ ] Image rotation
  * Need to check that x y angles are correct. Maybe we re-save the image properly in the DB?
  * Look briefly and if it's a huge pain in the ass ignore NEXT image (for now).
- [ ] REST calls from WPF
  * Will aim to query server-hosted DB via REST to get images/templates
    1. **Send** scanner name -> **Receive** image, capture guid
    2. **Send** template, capture guid -> **Receive** __nothing__
    3. **Send** scanner name -> **Receive** image, capture guid, template
  * Appropriate recording of API key (shouldn't save in source code)
- [x] Load templated capture
  * Review template
  * Update template
